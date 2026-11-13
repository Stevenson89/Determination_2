using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;
using Waylith.Math;

namespace Determination
{
    public class RitualOutcomeComp_LV : RitualOutcomeComp_Quality
    {
        [NoTranslate]
        public string? roleId;

        public override float QualityOffset(LordJob_Ritual ritual, RitualOutcomeComp_Data data)
        {
            return curve.Evaluate(Count(ritual, data));
        }

        public override float Count(LordJob_Ritual ritual, RitualOutcomeComp_Data data)
        {
            Pawn defendant = ritual.PawnWithRole(roleId);
            if (defendant == null)
                return 0f;
            int LV = defendant.TryGetComp<CompSoul>().LV;
            return LV;
        }
        public override string? GetDesc(LordJob_Ritual ritual = null, RitualOutcomeComp_Data data = null)
        {
            if (ritual == null)
                return this.labelAbstract;
            Pawn? pawn = ritual?.PawnWithRole(roleId);
            if (pawn == null)
                return null;
            float x = pawn.TryGetComp<CompSoul>().LV;
            float f = curve.Evaluate(x);
            string str = (double)f < 0.0 ? "" : "+";
            return (string)(label.CapitalizeFirst().Formatted(pawn.Named("PAWN")) + ": " + "OutcomeBonusDesc_QualitySingleOffset".Translate((NamedArgument)(str + f.ToStringPercent())) + ".");
        }

        public override QualityFactor? GetQualityFactor(Precept_Ritual ritual, TargetInfo ritualTarget, RitualObligation obligation, RitualRoleAssignments assignments, RitualOutcomeComp_Data data)
        {
            Pawn pawn = assignments.FirstAssignedPawn(this.roleId);
            if (pawn == null)
                return null;
            float x = pawn.TryGetComp<CompSoul>().LV;
            float f = curve.Evaluate(x);
            return new QualityFactor()
            {
                label = (string)label.Formatted(pawn.Named("PAWN")),
                count = x.ToString(),
                qualityChange = "OutcomeBonusDesc_QualitySingleOffset".Translate((NamedArgument)f.ToStringWithSign("0.#%")).Resolve(),
                positive = (double)f >= 0.0,
                quality = f,
                priority = 0.0f
            };
        }

        public override bool Applies(LordJob_Ritual ritual) => true;
    }
}
