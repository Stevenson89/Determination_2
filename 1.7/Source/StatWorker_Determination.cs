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
            return 0.7f - mentalBreakThreshold;
        }
    }
}
