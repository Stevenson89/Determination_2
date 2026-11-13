using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace Determination
{
    public class RitualSpectatorFilter_None : RitualSpectatorFilter
    {
        public override bool Allowed(Pawn p)
        {
            return false;
        }
    }
}
