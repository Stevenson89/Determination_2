using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;

namespace Determination
{
    [DefOf]
    public static class SoulInteractionDefOf
    {
        public static InteractionDef DivineJudgement_Judge;
        static SoulInteractionDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(SoulInteractionDefOf));
    }
}
