using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace Determination
{
    public class HediffComp_SetDetermination : HediffComp
    {
        public HediffCompProperties_SetDetermination Props => (HediffCompProperties_SetDetermination)props;
    }

    public class HediffCompProperties_SetDetermination : HediffCompProperties
    {
        public required float level;
        public HediffCompProperties_SetDetermination()
        {
            compClass = typeof(HediffComp_SetDetermination);
        }
    }
}
