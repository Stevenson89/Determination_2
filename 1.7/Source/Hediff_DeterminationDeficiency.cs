using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Determination
{
    public class Hediff_DeterminationDeficiency : Hediff
    {
        public override bool CauseDeathNow()
        {
            if (LoadedModManager.GetMod<DeterminationMod>().GetSettings<DeterminationSettings>().FatalDeterminationDeficiency)
                return base.CauseDeathNow();
            else
                return false;
        }
        public override void PostTickInterval(int delta)
        {
            CompSoul soul = pawn.TryGetComp<CompSoul>();
            if (soul != null && pawn.IsHashIntervalTick(200, delta))
            {
                if (soul.HasSoul)
                {
                    Severity -= soul.Determination * (1f / 300f);
                }
                else
                {
                    Severity += 0.1f * (1f / 300f);
                }
            }
        }
    }
}
