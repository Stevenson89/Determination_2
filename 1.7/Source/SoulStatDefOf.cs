using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;

namespace Determination
{
    [DefOf]
    public static class SoulStatDefOf
    {
        public static StatDef ExpGain;
        public static StatDef Determination;
        public static StatDef KillingIntent;
        public static StatDef KillingIntentVulnerability;
        static SoulStatDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(SoulStatDefOf));
    }
}
