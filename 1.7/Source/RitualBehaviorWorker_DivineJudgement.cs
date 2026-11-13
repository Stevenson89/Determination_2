using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace Determination
{
    public class RitualBehaviorWorker_DivineJudgement : RitualBehaviorWorker
    {
        private int ticksSinceLastInteraction = -1;
        public const int SocialInteractionIntervalTicks = 700;
        public RitualBehaviorWorker_DivineJudgement()
        {
        }

        public RitualBehaviorWorker_DivineJudgement(RitualBehaviorDef def)
          : base(def)
        {
        }

        public override void Cleanup(LordJob_Ritual ritual)
        {

        }

        public override void PostCleanup(LordJob_Ritual ritual)
        {

        }

        public override void Tick(LordJob_Ritual ritual)
        {
            base.Tick(ritual);
            if (ritual.StageIndex == 0)
                return;
            if (ticksSinceLastInteraction == -1 || ticksSinceLastInteraction > 700)
            {
                ticksSinceLastInteraction = 0;
                Pawn recipient1 = ritual.PawnWithRole("judge");
                Pawn recipient2 = ritual.PawnWithRole("defendant");
                recipient1.interactions.TryInteractWith(recipient2, SoulInteractionDefOf.DivineJudgement_Judge);
            }
            else
                ++ticksSinceLastInteraction;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref ticksSinceLastInteraction, "ticksSinceLastInteraction", -1);
        }
    }
}
