using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace Determination
{
    public class IncidentWorker_GiveWanderingJudgeQuest : IncidentWorker_GiveQuest
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!ModsConfig.IdeologyActive)
                return false;
            if (!Faction.OfPlayer.ideos.PrimaryIdeo.HasPrecept(DefDatabase<PreceptDef>.GetNamed("DivineJudgement_Required")))
                return false;
            return base.CanFireNowSub(parms);
        }
    }
}
