using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Determination
{
    public class CompProperties_CompSoulContainer : CompProperties
    {
        public CompProperties_CompSoulContainer()
        {
            compClass = typeof(CompSoulContainer);
        }

        public CompProperties_CompSoulContainer(Type compClass) : base(compClass)
        {
            this.compClass = compClass;
        }
    }
}
