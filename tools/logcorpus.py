"""
Extracts raw MPM packets (after decryption) from *_parsed.log files produced by the Logparser.

Output format (one packet per line):
    <C|S> <hex bytes without spaces>\t<relative source file>

C = Client -> World, S = World -> Client. Identical packets are only written once.

Usage:
    python logcorpus.py <logs root> <output file> [--max-per-file N]
"""
import argparse
import os
import sys


def iter_log_packets(path):
    """Yields (direction, hexstring) for every packet of one parsed log."""
    direction = None
    expect_hex = False
    with open(path, 'rb') as raw:
        head = raw.read(2)
        encoding = 'utf-16' if head in (b'\xff\xfe', b'\xfe\xff') else 'utf-8'
    with open(path, 'r', encoding=encoding, errors='replace') as f:
        for line in f:
            line = line.strip()
            if line == 'Client -> World':
                direction, expect_hex = 'C', True
                continue
            if line == 'World -> Client':
                direction, expect_hex = 'S', True
                continue
            if expect_hex:
                expect_hex = False
                parts = line.split(' ')
                if parts and all(len(p) == 2 for p in parts):
                    try:
                        bytes.fromhex(''.join(parts))
                    except ValueError:
                        continue
                    yield direction, ''.join(parts)


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument('root')
    ap.add_argument('output')
    ap.add_argument('--max-per-file', type=int, default=0)
    args = ap.parse_args()

    seen = set()
    written = 0
    with open(args.output, 'w', encoding='utf-8') as out:
        for dirpath, _, files in os.walk(args.root):
            for name in sorted(files):
                if not name.endswith('_parsed.log'):
                    continue
                full = os.path.join(dirpath, name)
                rel = os.path.relpath(full, args.root)
                count = 0
                try:
                    for direction, hexstr in iter_log_packets(full):
                        key = direction + hexstr
                        if key in seen:
                            continue
                        seen.add(key)
                        out.write('%s %s\t%s\n' % (direction, hexstr, rel))
                        written += 1
                        count += 1
                        if args.max_per_file and count >= args.max_per_file:
                            break
                except OSError as ex:
                    print('skip %s: %s' % (rel, ex), file=sys.stderr)
                print('%6d %s' % (count, rel), file=sys.stderr)
    print('packets written: %d' % written, file=sys.stderr)


if __name__ == '__main__':
    main()
