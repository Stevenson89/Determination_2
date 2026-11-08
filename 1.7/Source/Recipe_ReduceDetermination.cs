using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Determination
{
    public class Recipe_ReduceDetermination : Recipe_Surgery
    {
        private const float BloodLoss = 0.4f;

        public override bool CompletableEver(Pawn surgeryTarget)
        {
            return base.CompletableEver(surgeryTarget) && PawnHasEnoughBloodForRemoval(surgeryTarget);
        }

        public override void CheckForWarnings(Pawn medPawn)
        {
            base.CheckForWarnings(medPawn);
            if (PawnHasEnoughBloodForRemoval(medPawn))
                return;
            Messages.Message((string)"MessageCannotStartDeterminationReduction".Translate(medPawn.Named("PAWN")), (LookTargets)medPawn, MessageTypeDefOf.NeutralEvent, false);
        }

        public override void ApplyOnPawn(
          Pawn pawn,
          BodyPartRecord part,
          Pawn billDoer,
          List<Thing> ingredients,
          Bill bill)
        {
            Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.BloodLoss, pawn);
            hediff.Severity = BloodLoss;
            pawn.health.AddHediff(hediff);
            OnSurgerySuccess(pawn, part, billDoer, ingredients, bill);
        }
        protected override void OnSurgerySuccess(
          Pawn pawn,
          BodyPartRecord part,
          Pawn billDoer,
          List<Thing> ingredients,
          Bill bill)
        {
            Hediff monsterOverload = pawn.health.hediffSet.GetFirstHediffOfDef(SoulHediffDefOf.MonsterDeterminationOverload);
            Hediff overload = monsterOverload ?? pawn.health.hediffSet.GetFirstHediffOfDef(SoulHediffDefOf.DeterminationOverload);
            if (overload != null)
            {
                overload.Severity -= 1f;
            }
        }

        private bool PawnHasEnoughBloodForRemoval(Pawn pawn)
        {
            Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss);
            return firstHediffOfDef == null || (double)firstHediffOfDef.Severity < 1 - BloodLoss;
        }
    }
}
