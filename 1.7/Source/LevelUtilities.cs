using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Determination
{
    public static class LevelUtilities
    {
        public static Dictionary<int, int> Custom_LVToEXP = new Func<Dictionary<int, int>>(() =>
        {
            var dict = new Dictionary<int, int>();
            dict[0] = 0;
            for (int i = 1; i <= 10; i++)
            {
                dict[i] = (i - 1) * 500;
            }
            for (int i = 11; i <= 20; i++)
            {
                dict[i] = (i - 1) * 1000;
            }
            return dict;
        })();
        public static int GetStartingLV(Pawn pawn) // This is meant to reflect how many people this pawn has killed prior to being generated, not necessarily how capable of fighting the pawn currently is.
        {
            if (pawn.WorkTagIsDisabled(WorkTags.Violent))
            {
                return 0; // Pawns with no violent capabilities whatsoever have a special LV of 0. They are worth a single EXP when killed.
            }
            if (pawn.skills == null || pawn.story == null)
                return 1;
            int meleeSkill = pawn.skills.GetSkill(SkillDefOf.Melee).levelInt;
            int shootingSkill = pawn.skills.GetSkill(SkillDefOf.Shooting).levelInt;
            int meleeBonusFromBackstory = 0;
            int shootingBonusFromBackstory = 0;
            foreach (BackstoryDef backstory in pawn.story.AllBackstories)
            {
                SkillGain meleeGain = backstory.skillGains.Find(s => s.skill == SkillDefOf.Melee);
                SkillGain shootingGain = backstory.skillGains.Find(s => s.skill == SkillDefOf.Shooting);
                if (meleeGain != null) meleeBonusFromBackstory += meleeGain.amount;
                if (shootingGain != null) shootingBonusFromBackstory += shootingGain.amount;
            }
            int bloodlustBonus = pawn.story.traits.HasTrait(TraitDefOf.Bloodlust) ? 5 : 0;
            float LV = (meleeSkill + shootingSkill + meleeBonusFromBackstory + shootingBonusFromBackstory + bloodlustBonus) / 5; // The maximum value here is a starting level of 16. (Or 17 with Bloodlust.)
            return Math.Max(1, Mathf.FloorToInt(LV));
        }
        private static bool IsTemporaryInjury(Hediff h)
        {
            if (h is Hediff_Injury)
                return true;
            if (h.def == HediffDefOf.BloodLoss)
                return true;
            if (h.def == HediffDefOf.ToxGasExposure)
                return true;
            if (h.ageTicks < 10000)
                return true;
            return false;
        }

        public static float GetCapacityIgnoringInjuries(Pawn pawn, PawnCapacityDef capacity)
        {
            var hediffSet = pawn.health.hediffSet;
            var tempHediffs = hediffSet.hediffs
                .Where(h => IsTemporaryInjury(h))
                .ToList();

            foreach (var h in tempHediffs)
                hediffSet.hediffs.Remove(h);

            hediffSet.DirtyCache();

            float result = pawn.health.capacities.GetLevel(capacity);

            foreach (var h in tempHediffs)
                hediffSet.hediffs.Add(h);

            hediffSet.DirtyCache();
            return result;
        }

        public static float CalculateCombatStrength(Pawn pawn)
        {
            if (pawn == null) return 0f;

            // Armor / Defense
            float armorSharp = pawn.GetStatValue(StatDefOf.ArmorRating_Sharp);
            float armorBlunt = pawn.GetStatValue(StatDefOf.ArmorRating_Blunt);
            float armorScore = (armorSharp * 0.7f) + (armorBlunt * 0.3f);

            float incomingDmgFactor = pawn.GetStatValue(StatDefOf.IncomingDamageFactor);
            float damageReduction = 1f / Mathf.Max(incomingDmgFactor, 0.01f);

            float defensiveScore = (armorScore + damageReduction) * 0.5f;

            //  Weapon / Offense
            Thing weapon = pawn.equipment?.Primary;
            bool hasMeleeWeapon = weapon != null && weapon.def.IsMeleeWeapon;
            bool hasRangedWeapon = weapon != null && weapon.def.IsRangedWeapon;

            float weaponOffense = 0f;

            if (hasMeleeWeapon || !hasRangedWeapon)
            {
                weaponOffense = pawn.GetStatValue(StatDefOf.MeleeDPS);
            }
            else if (hasRangedWeapon)
            {
                VerbProperties primaryVerb = weapon.def.Verbs?.FirstOrDefault(v => v.isPrimary);
                if (primaryVerb?.defaultProjectile != null)
                {
                    float damage = primaryVerb.defaultProjectile.projectile.GetDamageAmount(weapon);
                    float cooldown = weapon.GetStatValue(StatDefOf.RangedWeapon_Cooldown);
                    float warmup = primaryVerb.warmupTime;
                    weaponOffense = damage / Mathf.Max(cooldown + warmup, 0.01f);
                }
            }

            float weaponScore = Mathf.Clamp(weaponOffense / 15f, 0f, 2f);

            float skillScore = 0.5f;
            if (pawn.skills != null)
            {
                SkillRecord relevantSkill = hasMeleeWeapon || !hasRangedWeapon
                    ? pawn.skills.GetSkill(SkillDefOf.Melee)
                    : pawn.skills.GetSkill(SkillDefOf.Shooting);
                if (relevantSkill != null)
                    skillScore = relevantSkill.Level / 20f;
            }

            float consciousness = GetCapacityIgnoringInjuries(pawn, PawnCapacityDefOf.Consciousness);
            float manipulation = GetCapacityIgnoringInjuries(pawn, PawnCapacityDefOf.Manipulation);
            float moving = GetCapacityIgnoringInjuries(pawn, PawnCapacityDefOf.Moving);
            float sight = GetCapacityIgnoringInjuries(pawn, PawnCapacityDefOf.Sight);

            float capabilityScore = (consciousness * 0.35f)
                      + (manipulation * 0.30f)
                      + (moving * 0.20f)
                      + (sight * 0.15f);

            float combatStrength =
                  (weaponScore * 0.38f)
                + (skillScore * 0.23f)
                + (capabilityScore * 0.23f)
                + (defensiveScore * 0.13f);

            return combatStrength;
        }

        public static int GetEXPForKill(Pawn victim, Pawn killer)
        {
            CompSoul compSoul = victim.TryGetComp<CompSoul>();
            if (compSoul != null && compSoul.LV == 0)
                return 1; // Disincentivizes killing pawns without the ability to defend themselves
            float combatStrength = CalculateCombatStrength(victim);
            float playerFactionFactor = victim.IsColonist ? 2 : 1;
            float expGainFactor = killer.GetStatValue(SoulStatDefOf.ExpGain);
            return Mathf.RoundToInt(500f * combatStrength * playerFactionFactor * expGainFactor);
        }

        public static bool CanLevelUp(int LV, int EXP)
        {
            if (LV >= 20)
                return false;
            if (EXP >= Custom_LVToEXP[LV + 1])
                return true;
            return false;
        }
    }
}
