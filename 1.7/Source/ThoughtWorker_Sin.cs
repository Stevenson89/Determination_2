using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;
using Waylith.Math;

namespace Determination
{
    public class ThoughtWorker_Sin : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            CompSoul compSoul = p.TryGetComp<CompSoul>();
            if (compSoul == null)
                return false;
            if (compSoul.UnjudgedSins == 0)
                return false;
            int maxedUnjudged = Mathf.Min(compSoul.UnjudgedSins, 5);
            return ThoughtState.ActiveAtStage(maxedUnjudged - 1);
        }
    }
}
