"""
Builds data/ability_bonuses.txt: the passive bonuses an ability grants while it is loaded
(e.g. HyperJumpAbility -> SuperJumpAbility +3 per level).

The gameobjects dump (fullgoprops.txt) contains the value scales of the stat modifiers but lost the target
ability references, so the targets are learned from the packet logs: the original server sent the ability
load response (RPC 0x80B2: ability, level, slot) and the bonus messages (RPC 0x80BC) in the same packet.
Value formula: floor(ValueScale * level) + MinValue.

Output (tab separated): source_ability target_ability flags value_scale min_value duration fit
    fit = props (scale from fullgoprops), single (only one level observed) or grid (searched)

Usage:
    python ability_bonus_gen.py <corpus from logcorpus.py> <fullgoprops.txt> <output>
"""
import collections
import re
import struct
import sys

RX_PROP = re.compile(r'^\t\|([^/|]+)/(ValueScale|MinValue)\| : \|([^|]*)\|')
RX_INFO = re.compile(r'^AbilityInfo: .*"abilityId": (\d+)L?, ')


def load_value_scales(path):
    """ability id -> list of (ValueScale, MinValue) of all modifier groups"""
    result = {}
    current = None

    def finish(obj):
        if obj and 'aid' in obj:
            result[obj['aid']] = [(g.get('ValueScale', 0.0), int(g.get('MinValue', 0.0))) for g in obj['groups'].values()]

    with open(path, encoding='latin1') as f:
        for line in f:
            if line.startswith('ObjId:'):
                finish(current)
                current = {'groups': {}}
                continue
            if current is None:
                continue
            m = RX_PROP.match(line)
            if m:
                current['groups'].setdefault(m.group(1), {})[m.group(2)] = float(m.group(3))
                continue
            m = RX_INFO.match(line)
            if m:
                current['aid'] = int(m.group(1))
    finish(current)
    return result


def find_all(data, pattern):
    start = 0
    while True:
        i = data.find(pattern, start)
        if i < 0:
            return
        start = i + 1
        yield i


def collect_pairs(corpus):
    pairs = collections.defaultdict(lambda: collections.defaultdict(list))
    flags_seen = collections.defaultdict(collections.Counter)
    with open(corpus, encoding='utf-8') as f:
        for line in f:
            if line[0] != 'S':
                continue
            data = bytes.fromhex(line[2:].split('\t')[0])
            loads = {}
            for i in find_all(data, b'\x80\xb2'):
                if data[i - 1] == 0x08 and i + 8 <= len(data):
                    ability, level, _slot = struct.unpack_from('<HHH', data, i + 2)
                    loads[ability] = level
            if not loads:
                continue
            for i in find_all(data, b'\x80\xbc'):
                if data[i - 1] != 0x16 or i + 22 > len(data):
                    continue
                payload = data[i + 2:i + 22]
                source = struct.unpack_from('<I', payload, 5)[0]
                target = struct.unpack_from('<H', payload, 9)[0]
                value = struct.unpack_from('<i', payload, 11)[0]
                removed = payload[15]
                duration = struct.unpack_from('<f', payload, 16)[0]
                if removed or source not in loads:
                    continue
                pairs[source][target].append((loads[source], value, round(duration, 3)))
                flags_seen[(source, target)][payload[0]] += 1
    return pairs, flags_seen


def fits(scale, minimum, samples):
    return all(int(scale * level + 1e-6) + minimum == value for level, value, _ in samples)


def fit(scales, samples):
    for scale, minimum in scales:
        for m in sorted({0, minimum}, key=abs):
            if fits(scale, m, samples):
                return scale, m, 'props'
    positive = [(level, value) for level, value, _ in samples if level > 0]
    if positive and len({level for level, _ in positive}) == 1:
        level, value = positive[0]
        return round(value / level, 4), 0, 'single'
    for minimum in sorted(range(-200, 201), key=abs):
        for step in range(0, 1001):
            if fits(step / 100, minimum, samples):
                return step / 100, minimum, 'grid'
    return None


def main():
    corpus, props_path, output = sys.argv[1:4]
    scales = load_value_scales(props_path)
    pairs, flags_seen = collect_pairs(corpus)

    rows = []
    for source in sorted(pairs):
        for target, samples in sorted(pairs[source].items()):
            flags = flags_seen[(source, target)].most_common(1)[0][0]
            if flags not in (0x45, 0x55):
                continue  # 0x15/0x1d are combat tactic / effect bonuses, not granted by loading an ability
            samples = sorted(set(samples))
            result = fit(scales.get(source, []), samples)
            if result is None:
                print('no fit for %d -> %d: %s' % (source, target, samples[:5]), file=sys.stderr)
                continue
            duration = collections.Counter(d for _, _, d in samples).most_common(1)[0][0]
            rows.append((source, target, flags, result[0], result[1], duration, result[2]))

    with open(output, 'w', encoding='utf-8', newline='\n') as out:
        out.write('# Generated by tools/ability_bonus_gen.py - passive bonuses of loaded abilities\n')
        out.write('# source\ttarget\tflags\tvalue_scale\tmin_value\tduration\tfit\n')
        for row in rows:
            out.write('%d\t%d\t%02x\t%s\t%d\t%s\t%s\n' % row)
    print('%d bonuses written' % len(rows), file=sys.stderr)


if __name__ == '__main__':
    main()
