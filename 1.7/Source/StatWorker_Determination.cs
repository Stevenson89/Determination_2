using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Determination
{
    public class StatWorker_Determination : StatWorker
    {
        public override float GetBaseValueFor(StatRequest request)
        {
            float mentalBreakThreshold = request.Thing.GetStatValue(StatDefOf.MentalBreakThreshold);
            float determinationOverride = -1f;
            foreach (Hediff hediff in request.Pawn.health.hediffSet.hediffs)
            {
                HediffComp_SetDetermination comp = hediff.TryGetComp<HediffComp_SetDetermination>();
                if (comp != null && comp.Props.level > determinationOverride)
                {
                    determinationOverride = comp.Props.level;
                }
            }
            if (determinationOverride >= -0f)
            {
                return determinationOverride;
            }
            return 0.7f - mentalBreakThreshold;
        }
    }
}
