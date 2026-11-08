using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Determination
{
    public class DeterminationSettings : ModSettings
    {
        //public bool AutoAssignMonsters = true;
        public bool FatalDeterminationDeficiency = true;
        public float KillingIntentFactor = 1f;
        public override void ExposeData()
        {
            //Scribe_Values.Look(ref AutoAssignMonsters, "AutoAssignMonsters");
            Scribe_Values.Look(ref FatalDeterminationDeficiency, "FatalDeterminationDeficiency");
            Scribe_Values.Look(ref KillingIntentFactor, "KillingIntentFactor");
            base.ExposeData();
        }
    }
}
