using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Determination
{
    public class CompProperties_Disintegrate: CompProperties
    {
        public CompProperties_Disintegrate()
        {
            compClass = typeof(CompDisintegrate);
        }

        public CompProperties_Disintegrate(Type compClass) : base(compClass)
        {
            this.compClass = compClass;
        }
    }
}
