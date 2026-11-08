using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Determination
{
    public class CompProperties_CompSoul :  CompProperties
    {
        public CompProperties_CompSoul()
        {
            compClass = typeof(CompSoul);
        }

        public CompProperties_CompSoul(Type compClass) : base(compClass)
        {
            this.compClass = compClass;
        }
    }
}
