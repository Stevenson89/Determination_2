using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace Determination
{
    public class RitualRoleJudgementDefendant : RitualRoleColonist
    {
        public override bool AppliesToPawn(Pawn p, out string reason, TargetInfo selectedTarget, LordJob_Ritual ritual = null, RitualRoleAssignments assignments = null, Precept_Ritual precept = null, bool skipReason = false)
        {
            if (!base.AppliesToPawn(p, out reason, selectedTarget, ritual, assignments, precept, skipReason))
                return false;
            CompSoul compSoul = p.TryGetComp<CompSoul>();
            if (compSoul == null)
                return false;
            if (compSoul.JudgedAt >= compSoul.LV)
            {
                if (!skipReason)
                    reason = "DT_CantStartRitualSelectedPawnAlreadyJudged".Translate(p.Named("PAWN"));
                return false;
            }
            PreceptDef preceptDef = DefDatabase<PreceptDef>.GetNamed("DivineJudgement_Required");
            if (p.Ideo == null || !p.Ideo.HasPrecept(preceptDef))
            {
                if (!skipReason)
                    reason = "DT_PawnDoesNotBelieveInJudgement".Translate(p.Named("PAWN"));
                return false;
            }
            return true;
        }
    }
}
