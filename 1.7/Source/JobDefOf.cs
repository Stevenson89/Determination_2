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
    public static class Determination_JobDefOf
    {
        public static JobDef AbsorbSoul;
        public static JobDef ExtractSoul;
        public static JobDef AbsorbSoulFromContainer;
        public static JobDef ExtractFromSoul;
        static Determination_JobDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(Determination_JobDefOf));
    }
}
