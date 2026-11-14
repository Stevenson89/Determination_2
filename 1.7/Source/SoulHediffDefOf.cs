using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Determination
{
    [DefOf]
    public static class SoulHediffDefOf
    {
        public static HediffDef SoulLossShock;
        public static HediffDef DeadAndSoullessHidden;
        public static HediffDef HumanSoulAbsorbed;
        public static HediffDef DeterminationOverload;
        public static HediffDef DeterminationHangover;
        public static HediffDef DeterminationDeficiency;
        public static HediffDef MonsterDeterminationOverload;
        public static HediffDef FallenMonster;
        public static HediffDef BossMonsterSoulAbsorbed;
        public static HediffDef RebellionShock;
        public static HediffDef SoulRebellion;
        [MayRequireIdeology]
        public static HediffDef KarmaBuildup;
        static SoulHediffDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(SoulHediffDefOf));
    }
}
