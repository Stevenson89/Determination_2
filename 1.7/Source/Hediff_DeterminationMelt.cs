using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Determination
{
    public class Hediff_DeterminationMelt : Hediff
    {
        public override void PostTickInterval(int delta)
        {
            Hediff overload = pawn.health.hediffSet.GetFirstHediffOfDef(SoulHediffDefOf.MonsterDeterminationOverload);
            if ((overload == null || overload.CurStageIndex == 2) && Severity < 1f)
            {
                Severity = 0f;
            }
            else if (pawn.IsHashIntervalTick(200, delta))
            {
                Severity += 5 * (1f / 300f);
            }
        }
    }
}
