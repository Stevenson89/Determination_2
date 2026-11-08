using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Determination
{
    [DefOf]
    public class SoulResearchDefOf
    {
        public static ResearchProjectDef SoulBasics;
        public static ResearchProjectDef DeterminationExtraction;

        static SoulResearchDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(SoulResearchDefOf));
    }
}
