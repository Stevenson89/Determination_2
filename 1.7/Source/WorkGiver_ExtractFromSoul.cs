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
    public class WorkGiver_ExtractFromSoul : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest =>
            ThingRequest.ForGroup(ThingRequestGroup.HaulableEver);

        public override PathEndMode PathEndMode => PathEndMode.ClosestTouch;

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            // cheap early-out: if there are no workstations of this type on the map, skip
            return !pawn.Map.listerBuildings.ColonistsHaveBuilding(ThingDefOf.DeterminationExtractor);
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            CompSoulContainer container = t.TryGetComp<CompSoulContainer>();
            if (container == null)
                return false;

            if (t.IsForbidden(pawn) || !pawn.CanReserve(t))
                return false;

            if (!container.CanExtract)
                return false;

            if (!container.extractAllowed)
                return false;

            Thing station = FindWorkstation(pawn, t);
            return station != null;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Thing station = FindWorkstation(pawn, t);
            if (station == null) return null;

            Job job = JobMaker.MakeJob(Determination_JobDefOf.ExtractFromSoul, t, station);
            job.count = 1;
            return job;
        }

        private Thing FindWorkstation(Pawn pawn, Thing item)
        {
            return GenClosest.ClosestThingReachable(
                pawn.Position,
                pawn.Map,
                ThingRequest.ForDef(ThingDefOf.DeterminationExtractor),
                PathEndMode.InteractionCell,
                TraverseParms.For(pawn),
                9999f,
                validator: station =>
                    !station.IsForbidden(pawn)
                    && pawn.CanReserve(station)
                    && station.Map.reachability.CanReach(item.Position, station, PathEndMode.InteractionCell, TraverseParms.For(pawn))
            );
        }
    }
}
