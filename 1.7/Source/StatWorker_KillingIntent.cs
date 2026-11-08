using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Determination
{
    public class StatWorker_KillingIntent : StatWorker
    {
        public override float GetBaseValueFor(StatRequest request)
        {
            Pawn pawn = request.Thing as Pawn;
            if (pawn == null)
                return 0f;
            CompSoul soul = pawn.TryGetComp<CompSoul>();
            if (soul == null)
                return 0f;
            float baseDetermination = soul.Determination;
            float tempDetermination = soul.TemporaryDetermination;
            float LV = soul.LV;
            return (baseDetermination + tempDetermination) * (LV / 10f);
        }
    }
}
