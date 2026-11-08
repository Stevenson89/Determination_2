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
    public class Hediff_DeterminationOverload : HediffWithComps
    {
        public float HighestSeverity = 0f;
        const float RecoverDeficiencyRate = 0.1f;
        public override string SeverityLabel
        {
            get => (double)Severity <= 0.0 ? null : Severity.ToStringPercent("F0");
        }
        public override float Severity {
            get => base.Severity;
            set {
                base.Severity = value;
                if (value > HighestSeverity)
                {
                    HighestSeverity = value;
                }
            } 
        }
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            if (SoulUtilities.IsMonsterForDetermination(pawn))
            {
                Hediff existingHediff = pawn.health.hediffSet.GetFirstHediffOfDef(SoulHediffDefOf.MonsterDeterminationOverload);
                if (existingHediff != null)
                {
                    existingHediff.Severity += Severity;
                }
                else
                {
                    Hediff newHediff = pawn.health.AddHediff(SoulHediffDefOf.MonsterDeterminationOverload);
                    newHediff.Severity = Severity;
                }
                HighestSeverity = 0;
                Severity = 0;
            }
        }
        public override void PostRemoved()
        {
            base.PostRemoved();
            if (HighestSeverity > 0f)
            {
                Hediff existing = pawn.health.hediffSet.GetFirstHediffOfDef(SoulHediffDefOf.DeterminationHangover);
                if (existing != null)
                {
                    existing.Severity += HighestSeverity;
                }
                else
                {
                    Hediff hangover = pawn.health.AddHediff(SoulHediffDefOf.DeterminationHangover);
                    hangover.Severity = HighestSeverity;
                }
            }
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
                HighestSeverity -= change; // Don't induce hangover from this
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref HighestSeverity, "HighestSeverity");
        }

        public override string DebugString()
        {
            return base.DebugString() + $"\nhighest severity: {HighestSeverity}".Indented();
        }
    }
}
