using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Determination
{
    public class CompProperties_CompExtractableSoul : CompProperties
    {
        public CompProperties_CompExtractableSoul()
        {
            compClass = typeof(CompExtractableSoul);
        }

        public CompProperties_CompExtractableSoul(Type compClass) : base(compClass)
        {
            this.compClass = compClass;
        }
    }
}
