using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Determination
{
    [HarmonyPatch(typeof(Thing), nameof(Thing.TakeDamage))]
    public class Patch_ApplyKillingIntent
    {
        static bool Prefix(Thing __instance, ref DamageInfo dinfo)
        {
            if (dinfo.Instigator == null)
                return true;
            float instigatorIntent = dinfo.Instigator.GetStatValue(SoulStatDefOf.KillingIntent);
            float victimVulnerability = __instance.GetStatValue(SoulStatDefOf.KillingIntentVulnerability);
            if (instigatorIntent != SoulStatDefOf.KillingIntent.defaultBaseValue && victimVulnerability > 0f)
            {
                float multiplier = LoadedModManager.GetMod<DeterminationMod>().GetSettings<DeterminationSettings>().KillingIntentFactor;
                float offset = instigatorIntent * victimVulnerability * multiplier;
                float current = dinfo.Amount;
                if (offset != 0f)
                {
                    if (current + offset <= 0f)
                        dinfo.SetAmount(0f);
                    else
                        dinfo.SetAmount(current + offset);
                }
            }
            return true;
        }
    }
}
