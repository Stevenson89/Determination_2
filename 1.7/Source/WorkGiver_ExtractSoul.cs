using Determination;
using RimWorld;
using Verse;
using Verse.AI;

namespace Determination
{
    public class WorkGiver_ExtractSoul : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest =>
            ThingRequest.ForGroup(ThingRequestGroup.Corpse);

        public override PathEndMode PathEndMode => PathEndMode.Touch;

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            return !pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Corpse)
                .Any(t => t.TryGetComp<CompExtractableSoul>()?.ExtractionRequested == true);
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            var comp = t.TryGetComp<CompExtractableSoul>();
            if (comp == null || !comp.ExtractionRequested) return false;

            var container = comp.extractionTarget.Thing;
            if (container == null || container.Destroyed) return false;

            var containerComp = container.TryGetComp<CompSoulContainer>();
            if (containerComp == null || containerComp.HasSoul) return false;

            if (t.IsForbidden(pawn)) return false;
            if (!pawn.CanReserve(t, 1, -1, null, forced)) return false;
            if (!pawn.CanReserve(container, 1, -1, null, forced)) return false;
            if (!pawn.CanReach(t, PathEndMode.Touch, Danger.Some)) return false;
            if (!pawn.CanReach(container, PathEndMode.Touch, Danger.Some)) return false;

            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            var comp = t.TryGetComp<CompExtractableSoul>();
            Job job = JobMaker.MakeJob(Determination_JobDefOf.ExtractSoul, t, comp.extractionTarget);
            job.count = 1;
            return job;
        }
    }
}