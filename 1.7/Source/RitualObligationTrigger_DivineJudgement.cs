using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace Determination
{
    public class RitualObligationTrigger_DivineJudgement : RitualObligationTrigger_EveryMember
    {
        private static List<Pawn> existingObligations = new List<Pawn>();
        protected override void Recache()
        {
            try
            {
                if (ritual.activeObligations != null)
                {
                    ritual.activeObligations.RemoveAll((Predicate<RitualObligation>)(o => o.targetA.Thing is Pawn thing && thing.Ideo != this.ritual.ideo));
                    foreach (RitualObligation activeObligation in this.ritual.activeObligations)
                        existingObligations.Add(activeObligation.targetA.Thing as Pawn);
                }
                foreach (Pawn transportersAliveColonist in PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_Colonists)
                {
                    if (!existingObligations.Contains(transportersAliveColonist) && transportersAliveColonist.Ideo != null && transportersAliveColonist.Ideo == this.ritual.ideo && !transportersAliveColonist.IsPrisoner && transportersAliveColonist.health.hediffSet.GetNotMissingParts().Any<BodyPartRecord>((Func<BodyPartRecord, bool>)(p => p.def == BodyPartDefOf.Eye)))
                        this.ritual.AddObligation(new RitualObligation(this.ritual, (TargetInfo)(Thing)transportersAliveColonist, false));
                }
            }
            finally
            {
                existingObligations.Clear();
            }
        }
    }

    public class RitualObligationTriggerProperties_DivineJudgement : RitualObligationTriggerProperties
    {
        public RitualObligationTriggerProperties_DivineJudgement()
        {
            triggerClass = typeof(RitualObligationTrigger_DivineJudgement);
        }
    }
}
