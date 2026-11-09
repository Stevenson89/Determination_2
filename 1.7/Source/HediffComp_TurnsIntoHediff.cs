using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace Determination
{
    public class HediffComp_TurnsIntoHediff : HediffComp
    {
        public HediffCompProperties_TurnsIntoHediff Props => (HediffCompProperties_TurnsIntoHediff)props;
        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            Pawn.health.AddHediff(Props.intoHediff);
        }
    }

    public class HediffCompProperties_TurnsIntoHediff : HediffCompProperties
    {
        public required HediffDef intoHediff;
        public HediffCompProperties_TurnsIntoHediff()
        {
            compClass = typeof(HediffComp_TurnsIntoHediff);
        }
    }
}
