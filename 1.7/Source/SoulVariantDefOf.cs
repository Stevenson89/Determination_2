using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;

namespace Determination
{
    [DefOf]
    public static class SoulVariantDefOf
    {
        public static SoulVariantDef SoullessHuman;
        public static SoulVariantDef SoullessMonster;
        static SoulVariantDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(SoulVariantDefOf));
    }
}
