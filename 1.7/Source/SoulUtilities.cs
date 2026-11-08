using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Waylith.Math;

namespace Determination
{
    public static class SoulUtilities
    {
        const int MAX_BOSS_ABSORPTION = 3;
        const int MAX_HUMAN_ABSORPTION = 100;
        public static bool IsMonster(Pawn pawn)
        {
            if (pawn.genes == null)
                return false;
            return pawn.genes.GenesListForReading.Any(g => g.def.HasModExtension<DeterminationModExtension>() && g.def.GetModExtension<DeterminationModExtension>().monsterSoul == true);
        }
        public static bool IsBossMonster(Pawn pawn)
        {
            if (pawn.genes?.GenesListForReading?.ContainsAny(gene => gene.def.GetModExtension<DeterminationModExtension>()?.bossMonster == true) == true)
            {
                return true;
            }
            return false;
        }
        public static bool IsMonsterForDetermination(Pawn pawn)
        {
            if (pawn.genes == null)
                return false;
            return pawn.genes.GenesListForReading.Any(g => g.def.GetModExtension<DeterminationModExtension>()?.monsterDeterminationOverload == true);
        }
        public static Soul ExtractSoul(Pawn pawn)
        {
            Soul result = new Soul(pawn);
            MakeSoulless(pawn);
            return result;
        }
        public static SoulVariantDef GetSoul(bool monster, bool isBoss)
        {
            if (isBoss)
                return SoulVariantDefOf.BossMonster;
            SoulColorDef color = DefDatabase<SoulColorDef>.AllDefs.Where(s => s.monster == monster && s.canSpawn == true).RandomElement();
            IEnumerable<SoulVariantDef> candidates = DefDatabase<SoulVariantDef>.AllDefs.Where(d => d.type == color && d.soulless == false);
            return candidates.RandomElement();
        }
        public static void MakeSoulless(Pawn pawn)
        {
            CompSoul targetSoul = pawn.TryGetComp<CompSoul>();
            if (targetSoul != null)
            {
                if (targetSoul.Variant.type.monster)
                {
                    targetSoul.Variant = SoulVariantDefOf.SoullessMonster;
                }
                else
                {
                    targetSoul.Variant = SoulVariantDefOf.SoullessHuman;
                }
                if (pawn.Dead)
                {
                    pawn.health.AddHediff(SoulHediffDefOf.DeadAndSoullessHidden);
                }
                else
                {
                    DoSoulLossSideEffects(pawn, true);
                }
            }
            else
                Log.Warning("Attempted to absorb soul of a pawn without a CompSoul");
            // Can this happen if the mod is added mid-save?
        }
        public static void DoSoulLossSideEffects(Pawn pawn, bool shock)
        {
            if (shock)
                pawn.health.AddHediff(SoulHediffDefOf.SoulLossShock);
            pawn.story.traits.GainTrait(new Trait(TraitDefOf.Psychopath), true);
        }
        public static void IncreasePawnSouls(Pawn pawn)
        {
            if (IsMonster(pawn))
            {
                Hediff existing = pawn.health.hediffSet.GetFirstHediffOfDef(SoulHediffDefOf.HumanSoulAbsorbed);
                if (existing != null)
                {
                    existing.Severity += 1;
                }
                else
                {
                    pawn.health.AddHediff(SoulHediffDefOf.HumanSoulAbsorbed);
                }
                pawn.health.hediffSet.DirtyCache();
                pawn.needs.AddOrRemoveNeedsAsAppropriate();
            }
            else
            {
                Hediff existing = pawn.health.hediffSet.GetFirstHediffOfDef(SoulHediffDefOf.BossMonsterSoulAbsorbed);
                if (existing != null)
                {
                    existing.Severity += 1;
                }
                else
                {
                    pawn.health.AddHediff(SoulHediffDefOf.BossMonsterSoulAbsorbed);
                }
                pawn.health.hediffSet.DirtyCache();
                pawn.needs.AddOrRemoveNeedsAsAppropriate();
            }
        }
        public static void UpdatePawnSoulHediff(Pawn pawn)
        {
            CompSoul compSoul = pawn.TryGetComp<CompSoul>();
            if (compSoul == null)
                return;
            bool monster = IsMonster(pawn);
            float absorptionCap = monster ? MAX_HUMAN_ABSORPTION : MAX_BOSS_ABSORPTION;
            float targetSeverity = Mathf.Clamp(compSoul.ExtraSouls.Count, 0, absorptionCap);
            
            HediffDef absorptionHediff = monster ? SoulHediffDefOf.HumanSoulAbsorbed : SoulHediffDefOf.BossMonsterSoulAbsorbed;
            

            Hediff? existing = pawn.health.hediffSet.GetFirstHediffOfDef(absorptionHediff);
            float severity = existing?.Severity ?? 0;
            if (severity == 0 && targetSeverity > 0)
            {
                Hediff hediff = pawn.health.AddHediff(absorptionHediff);
                hediff.Severity = targetSeverity;
            }
            else
            {
                existing?.Severity = targetSeverity;
            }
              
        }

        public static float GetDeterminationExhaustion(Pawn pawn)
        {
            Hediff overload;
            if (!IsMonsterForDetermination(pawn))
                overload = pawn.health.hediffSet.GetFirstHediffOfDef(SoulHediffDefOf.DeterminationOverload);
            else
                overload = pawn.health.hediffSet.GetFirstHediffOfDef(SoulHediffDefOf.MonsterDeterminationOverload);
            Hediff hangover = pawn.health.hediffSet.GetFirstHediffOfDef(SoulHediffDefOf.DeterminationHangover);
            
            float exhaustion = (overload != null ? overload.Severity : 0) + (hangover != null ? hangover.Severity : 0);
            return exhaustion;
        }

        public static bool CanPawnSelfRevive(Pawn pawn)
        {
            CompSoul soul = pawn.TryGetComp<CompSoul>();
            if (soul == null)
                return false;

            float determination = soul.Determination;

            if (determination < 0.5)
                return false;

            float exhaustion = GetDeterminationExhaustion(pawn);

            if (exhaustion >= determination)
                return false;

            return true;
        }

        public static void AddDeterminationExhaustion(Pawn pawn, float exhaustion)
        {
            Hediff overload = pawn.health.hediffSet.GetFirstHediffOfDef(SoulHediffDefOf.DeterminationOverload);
            if (overload == null)
            {
                overload = pawn.health.AddHediff(SoulHediffDefOf.DeterminationOverload);
                overload.Severity = exhaustion;
            }
            else
            {
                overload.Severity += exhaustion;
            }
        }

        public static bool CanAbsorbTargetSoul(Pawn absorber, Pawn deceased)
        {
            if (absorber == null) return false;
            if (deceased == null) return false;
            if (deceased.TryGetComp<CompSoul>().HasSoul == false) return false;
            if (IsMonster(absorber))
            {
                return !IsMonster(deceased);
            }
            else
            {
                Hediff hediff = absorber.health.hediffSet.GetFirstHediffOfDef(SoulHediffDefOf.BossMonsterSoulAbsorbed);
                int alreadyAbsorbed = (int)(hediff != null ? hediff.Severity : 0);
                if (alreadyAbsorbed >= MAX_BOSS_ABSORPTION)
                    return false;
                return IsBossMonster(deceased);
            }
        }

        public static bool CanAbsorbSoul(Pawn absorber, SoulVariantDef soul)
        {
            if (IsMonster(absorber))
            {
                return !soul.type.monster;
            }
            else if (soul == SoulVariantDefOf.BossMonster)
            {
                Hediff hediff = absorber.health.hediffSet.GetFirstHediffOfDef(SoulHediffDefOf.BossMonsterSoulAbsorbed);
                int alreadyAbsorbed = (int)(hediff != null ? hediff.Severity : 0);
                if (alreadyAbsorbed >= MAX_BOSS_ABSORPTION)
                    return false;
                return true;
            }
            return false;
        }
    }
}
