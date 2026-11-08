using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Determination
{
    public class Hediff_DeadAndSoullessHidden : Hediff
    {
        public override void Notify_Resurrected()
        {
            SoulUtilities.DoSoulLossSideEffects(pawn, false);
            Severity = 0f;
        }
    }
}
