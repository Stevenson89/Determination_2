using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Determination
{
    public class Hediff_MonsterDeterminationOverload : HediffWithComps
    {
        const float RecoverDeficiencyRate = 0.1f;
        public override string SeverityLabel
        {
            get => (double)Severity <= 0.0 ? null : Severity.ToStringPercent("F0");
        }
        public override void PostTickInterval(int delta)
        {
            base.PostTickInterval(delta);
            Hediff deficiency = pawn.health.hediffSet.GetFirstHediffOfDef(SoulHediffDefOf.DeterminationDeficiency);
            if (Severity > 0f && deficiency != null && deficiency.Severity > 0f)
            {
                float change = RecoverDeficiencyRate * delta;
                if (deficiency.Severity - change <= 0)
                    change = deficiency.Severity;
                deficiency.Severity -= change;
                Severity -= change;
            }

            Hediff fallen = pawn.health.hediffSet.GetFirstHediffOfDef(SoulHediffDefOf.FallenMonster);
            if (Severity > 0.75f && fallen != null && fallen.Severity > 0f)
            {
                pawn.health.RemoveHediff(fallen);
            }
        }
    }
}
