using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace Determination
{
    public class JobDriver_AbsorbSoul : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);

            Toil doAction = new Toil
            {
                defaultCompleteMode = ToilCompleteMode.Delay,
                defaultDuration = 120 // ~2 in-game seconds
            };
            doAction.WithProgressBarToilDelay(TargetIndex.A);
            doAction.AddFinishAction(() =>
            {
                SoulUtilities.IncreasePawnSouls(pawn);
                SoulUtilities.MakeSoulless(((Corpse)TargetA.Thing).InnerPawn);
            });
            yield return doAction;
        }
    }
}
