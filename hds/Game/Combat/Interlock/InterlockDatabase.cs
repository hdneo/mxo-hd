using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace hds.Game.Combat.Interlock
{
    public sealed class InterlockMove
    {
        public int Id;
        /// <summary>E = exchange, T = style transition, O = other (escapes, opportunity attacks, special moves...)</summary>
        public char Kind;
        /// <summary>Exchange: style + tactic letters ("PD"); transition: style name.</summary>
        public string Attacker;
        public string Defender;
        /// <summary>Outcome part of the group name ("Hit:HM", "Killing", ...).</summary>
        public string Outcome;
        public int DurationMs;
        public string Group;
        public string Description;
    }

    /// <summary>Timing values the original server used for an exchange move (see tools/interlock_timing_gen.py).</summary>
    public sealed class InterlockTiming
    {
        public ushort TransitionTimeA, TransitionTimeB, Unknown3;
        public short ExchangeStart;
        public ushort DurationA, DurationB;
        public byte Flags;
        public ushort DamageScaleA, DamageScaleB;
        public uint TailA1, TailA2, TailB1, TailB2;
    }

    /// <summary>
    /// The interlock move database (data/interlock_moves.txt, generated from interlock_master.xml) plus the
    /// timing templates learned from the logs (data/interlock_timing.txt).
    /// </summary>
    public sealed class InterlockDatabase
    {
        /// <summary>Transition the original server used in the first round for most styles ("Trans AS-GB Draw").</summary>
        public const int DefaultDrawTransition = 0x5214;

        private readonly List<InterlockMove> moves = new List<InterlockMove>();
        private readonly Dictionary<(string, string, string), List<InterlockMove>> exchanges = new Dictionary<(string, string, string), List<InterlockMove>>();
        private readonly Dictionary<(string, string), List<InterlockMove>> transitions = new Dictionary<(string, string), List<InterlockMove>>();
        private readonly Dictionary<int, InterlockTiming> timings = new Dictionary<int, InterlockTiming>();

        private static InterlockDatabase instance;

        public static InterlockDatabase Instance => instance ??= Load(hds.Core.ServerPaths.Data("interlock_moves.txt"), hds.Core.ServerPaths.Data("interlock_timing.txt"));

        public int MoveCount => moves.Count;
        public int TimingCount => timings.Count;

        public static InterlockDatabase Load(string movesPath, string timingPath)
        {
            var db = new InterlockDatabase();
            using (var reader = new StreamReader(movesPath))
            {
                db.ParseMoves(reader);
            }
            if (timingPath != null && File.Exists(timingPath))
            {
                using var reader = new StreamReader(timingPath);
                db.ParseTimings(reader);
            }
            return db;
        }

        public void ParseMoves(TextReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Length == 0 || line[0] == '#') continue;
                var p = line.Split('\t');
                var move = new InterlockMove
                {
                    Id = int.Parse(p[0]), Kind = p[1][0], Attacker = p[2], Defender = p[3], Outcome = p[4],
                    DurationMs = int.Parse(p[5]), Group = p[7], Description = p[8],
                };
                if (move.Id != moves.Count) throw new InvalidDataException($"interlock move ids must be consecutive (got {move.Id})");
                moves.Add(move);
                if (move.Kind == 'E') Add(exchanges, (move.Attacker, move.Defender, move.Outcome), move);
                if (move.Kind == 'T') Add(transitions, (move.Attacker, move.Defender), move);
            }
        }

        public void ParseTimings(TextReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Length == 0 || line[0] == '#') continue;
                var v = line.Split('\t');
                timings[int.Parse(v[0])] = new InterlockTiming
                {
                    TransitionTimeA = ushort.Parse(v[2]), TransitionTimeB = ushort.Parse(v[3]), Unknown3 = ushort.Parse(v[4]),
                    ExchangeStart = short.Parse(v[5]), DurationA = ushort.Parse(v[6]), DurationB = ushort.Parse(v[7]),
                    Flags = byte.Parse(v[8]), DamageScaleA = ushort.Parse(v[9]), DamageScaleB = ushort.Parse(v[10]),
                    TailA1 = uint.Parse(v[11]), TailA2 = uint.Parse(v[12]), TailB1 = uint.Parse(v[13]), TailB2 = uint.Parse(v[14]),
                };
            }
        }

        private static void Add<TKey>(Dictionary<TKey, List<InterlockMove>> index, TKey key, InterlockMove move)
        {
            if (!index.TryGetValue(key, out var list))
            {
                list = new List<InterlockMove>();
                index[key] = list;
            }
            list.Add(move);
        }

        public InterlockMove Get(int id) => id >= 0 && id < moves.Count ? moves[id] : null;

        public bool TryGetTiming(int moveId, out InterlockTiming timing) => timings.TryGetValue(moveId, out timing);

        /// <summary>
        /// Picks an exchange move for the given styles, tactics and outcome. Falls back to other tactics and
        /// finally other melee styles if the exact combination does not exist. Moves the original server used
        /// (with timing data) are preferred.
        /// </summary>
        public InterlockMove FindExchange(CombatStyle attackerStyle, CombatTactic attackerTactic, CombatStyle defenderStyle,
            CombatTactic defenderTactic, ExchangeOutcome outcome, Random random)
        {
            var a = CombatNames.Letter(attackerStyle);
            var d = CombatNames.Letter(defenderStyle);
            var at = CombatNames.Letter(attackerTactic);
            var dt = CombatNames.Letter(defenderTactic);
            const string tactics = "SPDB";

            foreach (var outcomeGroup in CombatNames.OutcomeGroups(outcome))
            {
                var attempts = new List<(string, string)> {($"{a}{at}", $"{d}{dt}")};
                attempts.AddRange(tactics.Select(t => ($"{a}{at}", $"{d}{t}")));
                attempts.AddRange(tactics.SelectMany(t1 => tactics.Select(t2 => ($"{a}{t1}", $"{d}{t2}"))));
                foreach (var (attackerCode, defenderCode) in attempts)
                {
                    if (exchanges.TryGetValue((attackerCode, defenderCode, outcomeGroup), out var candidates))
                    {
                        return Pick(candidates, random);
                    }
                }
            }

            // style combination unknown: keep the outcome and the kind of styles - an unarmed fighter must never get
            // a weapon move (and the other way round)
            var attackerMelee = CombatNames.IsMelee(attackerStyle);
            var defenderMelee = CombatNames.IsMelee(defenderStyle);
            foreach (var outcomeGroup in CombatNames.OutcomeGroups(outcome))
            {
                var candidates = exchanges
                    .Where(e => e.Key.Item3 == outcomeGroup &&
                                IsMeleeLetter(e.Key.Item1[0]) == attackerMelee && IsMeleeLetter(e.Key.Item2[0]) == defenderMelee)
                    .SelectMany(e => e.Value).ToList();
                if (candidates.Count > 0) return Pick(candidates, random);
            }
            return null;
        }

        private static bool IsMeleeLetter(char styleLetter) => styleLetter == 'A' || styleLetter == 'W' || styleLetter == 'K' || styleLetter == 'S';

        /// <summary>
        /// Interlock move of a special move ability. Some abilities have one move per opponent style and tactic
        /// ("KarateAbility GuardBreaker vs WS"); the matching one is used, otherwise the first (main) move.
        /// </summary>
        public InterlockMove FindSpecialMove(SpecialMove special, CombatStyle defenderStyle, CombatTactic defenderTactic, Random random)
        {
            var code = $"vs {CombatNames.Letter(defenderStyle)}{CombatNames.Letter(defenderTactic)}";
            var candidates = special.MoveIds.Select(Get).Where(m => m != null).ToList();
            if (candidates.Count == 0) return null;
            var matching = candidates.Where(m => m.Description.EndsWith(code, StringComparison.Ordinal)).ToList();
            if (matching.Count > 0) return matching[random.Next(matching.Count)];
            var sameStyle = candidates.Where(m => m.Description.Contains($"vs {CombatNames.Letter(defenderStyle)}", StringComparison.Ordinal)).ToList();
            if (sameStyle.Count > 0) return sameStyle[random.Next(sameStyle.Count)];
            return candidates[0];
        }

        /// <summary>Transition move for a style change (null if the database has none).</summary>
        public InterlockMove FindTransition(CombatStyle from, CombatStyle to, Random random)
        {
            return transitions.TryGetValue((CombatNames.TransitionName(from), CombatNames.TransitionName(to)), out var list)
                ? Pick(list, random)
                : null;
        }

        /// <summary>
        /// Transition of the first round: the fighter draws into its style. Unarmed fighters of the original
        /// server got "Transitions from SubM:to &lt;style&gt;" (e.g. "Trans GP-K Draw"); the former default
        /// "Trans AS-GB Draw" goes the other way (into the SMG stance) and made unarmed players play gun moves.
        /// </summary>
        public InterlockMove FirstRoundTransition(CombatStyle style, Random random)
        {
            if (CombatNames.IsMelee(style))
            {
                return FindTransition(CombatStyle.SubMachineGun, style, random) ?? FindTransition(CombatStyle.Aikido, style, random);
            }
            return FindTransition(CombatStyle.Aikido, style, random) ?? Get(DefaultDrawTransition);
        }

        private InterlockMove Pick(List<InterlockMove> candidates, Random random)
        {
            var known = candidates.Where(m => timings.ContainsKey(m.Id)).ToList();
            var pool = known.Count > 0 ? known : candidates;
            return pool[random.Next(pool.Count)];
        }
    }
}
