using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace Determination
{
    public class JobDriver_ExtractSoulToContainer : JobDriver
    {
        private Corpse Corpse => job.targetA.Thing as Corpse;
        private Thing Container => job.targetB.Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(Corpse, job, 1, -1, null, errorOnFailed)
                && pawn.Reserve(Container, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnDestroyedOrNull(TargetIndex.B);

            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.Touch)
                .FailOnDespawnedNullOrForbidden(TargetIndex.B);

            Toil startCarryingContainer = Toils_Haul.StartCarryThing(TargetIndex.B);
            Toil carryToCorpse = Toils_Haul.CarryHauledThingToCell(TargetIndex.A);
            Toil extract = Toils_General.Wait(180)
                .WithProgressBarToilDelay(TargetIndex.A)
                .FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            Toil finish = Toils_General.Do(() =>
            {
                var corpseComp = Corpse.TryGetComp<CompExtractableSoul>();
                var containerComp = Container.TryGetComp<CompSoulContainer>();
                Soul soul = SoulUtilities.ExtractSoul(Corpse.InnerPawn);
                containerComp.soul = soul;
                containerComp.determination = 1f;
                corpseComp.extractionTarget = LocalTargetInfo.Invalid;
            });

            yield return startCarryingContainer;
            yield return carryToCorpse;
            yield return extract;
            yield return finish;
        }
    }
}
