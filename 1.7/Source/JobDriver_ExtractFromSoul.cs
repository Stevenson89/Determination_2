using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace Determination
{
    public class JobDriver_ExtractFromSoul : JobDriver
    {
        private Thing soulContainer => TargetA.Thing;
        private CompSoulContainer soul => soulContainer.TryGetComp<CompSoulContainer>();
        private Thing extractor => TargetB.Thing;
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            pawn.Reserve(soulContainer, job);
            pawn.Reserve(extractor, job);
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedNullOrForbidden(TargetIndex.B);
            this.FailOnDestroyedOrNull(TargetIndex.A);
            this.FailOn(() =>
            {
                CompPowerTrader power = extractor.TryGetComp<CompPowerTrader>();
                return power != null && !power.PowerOn;
            });

            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch)
                .FailOnDespawnedNullOrForbidden(TargetIndex.A); // Go to soul container
            yield return Toils_Haul.StartCarryThing(TargetIndex.A); // Pick up soul container
            yield return Toils_Haul.CarryHauledThingToCell(TargetIndex.B); // Carry to extractor
            Toil finish = Toils_General.Wait(180)
                .WithProgressBarToilDelay(TargetIndex.B)
                .FailOnCannotTouch(TargetIndex.B, PathEndMode.Touch);
            finish.AddFinishAction(() =>
            {
                soul.determination -= 1f;
                Thing determinationSyringe = ThingMaker.MakeThing(ThingDefOf.DeterminationSyringe);
                GenSpawn.Spawn(determinationSyringe, pawn.Position, pawn.Map);
            });
            yield return finish;
            
        }
    }
}
