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
    public static class ThingDefOf
    {
        public static ThingDef DeterminationExtractor;
        public static ThingDef DeterminationSyringe;
        static ThingDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(ThingDefOf));
    }
}
