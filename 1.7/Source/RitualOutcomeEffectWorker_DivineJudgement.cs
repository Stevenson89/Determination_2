using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace Determination
{
    public class RitualOutcomeEffectWorker_DivineJudgement : RitualOutcomeEffectWorker_FromQuality
    {
        public RitualOutcomeEffectWorker_DivineJudgement()
        {
        }

        public RitualOutcomeEffectWorker_DivineJudgement(RitualOutcomeEffectDef def)
          : base(def)
        {
        }
        public override void Apply(float progress, Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual)
        {
            float quality = GetQuality(jobRitual, progress);
            RitualOutcomePossibility outcome = GetOutcome(quality, jobRitual);
            LookTargets selectedTarget = (LookTargets)jobRitual.selectedTarget;
            Pawn judge = jobRitual.PawnWithRole("judge");
            Pawn defendant = jobRitual.PawnWithRole("defendant");

            CompSoul defendantCompSoul = defendant.TryGetComp<CompSoul>();
            if (defendantCompSoul != null)
            {
                defendantCompSoul.UnjudgedSins = 0;
                defendantCompSoul.JudgedAt = defendantCompSoul.LV;
            }
            GiveMemoryToPawn(defendant, outcome.memory, jobRitual);

            TaggedString taggedString = outcome.description.Formatted(defendant.Named("PAWN"), judge.Named("JUDGE"));
            TaggedString text = taggedString + ("\n\n" + OutcomeQualityBreakdownDesc(quality, progress, jobRitual));
            Find.LetterStack.ReceiveLetter("OutcomeLetterLabel".Translate(outcome.label.Named("OUTCOMELABEL"), jobRitual.Ritual.Label.Named("RITUALLABEL")), text, outcome.Positive ? LetterDefOf.RitualOutcomePositive : LetterDefOf.RitualOutcomeNegative, selectedTarget);
        }
    }
}
