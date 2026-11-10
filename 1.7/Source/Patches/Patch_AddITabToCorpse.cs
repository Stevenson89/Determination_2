using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Determination.Patches
{
    [HarmonyPatch(typeof(ThingDefGenerator_Corpses), "GenerateCorpseDef")]
    public class Patch_AddITabToCorpse
    {
        public static void Postfix(ThingDef __result)
        {
            if (!__result.comps.Exists(c => c.compClass == typeof(CompExtractableSoul)))
                __result.comps.Add(new CompProperties_CompExtractableSoul());

            if (!__result.comps.Exists(c => c.compClass == typeof(CompDisintegrate)))
                __result.comps.Add(new CompProperties_Disintegrate());

            if (!__result.inspectorTabs.Contains(typeof(ITab_Soul)))
                __result.inspectorTabs.Add(typeof(ITab_Soul));
        }
    }
}
