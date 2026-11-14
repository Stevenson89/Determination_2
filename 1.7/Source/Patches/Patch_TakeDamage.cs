using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Determination.Patches
{
    [HarmonyPatch(typeof(Thing), nameof(Thing.TakeDamage))]
    public class Patch_TakeDamage
    {
        static bool Prefix(Thing __instance, ref DamageInfo dinfo)
        {
            if (dinfo.Instigator == null)
                return true;
            Pawn? instigatorPawn = dinfo.Instigator as Pawn;
            Pawn? victimPawn = __instance as Pawn;
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
            if (ModsConfig.IdeologyActive && instigatorPawn?.Ideo != null && victimPawn != null)
            {
                CompSoul compSoul = victimPawn.TryGetComp<CompSoul>();
                if (compSoul == null)
                    return true;
                if (instigatorPawn.Ideo.GetRole(instigatorPawn)?.def?.roleEffects.ContainsAny(r => r.GetType() == typeof(RoleEffect_EnableKarma)) == true && compSoul.LV > 1)
                {
                    Hediff karma = victimPawn.health.GetOrAddHediff(SoulHediffDefOf.KarmaBuildup);
                    karma.Severity += (dinfo.Amount / 20) * compSoul.LV;
                }
            }
            return true;
        }
    }
}
