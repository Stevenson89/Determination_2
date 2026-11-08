using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Determination
{
    public class CompSoul : ThingComp
    {
        StringBuilder stringBuilder = new StringBuilder();
        public SoulVariantDef Variant;
        private int _EXP = 0;
        public float LVTolerance = 0f;
        private bool isbeingloaded = false;
        public Pawn pawn
        {
            get
            {
                return (Pawn)parent; 
            }
        }
        public bool HasSoul
        {
            get
            {
                return !Variant.soulless;
            }
        }
        public float Determination
        {
            get
            {
                return parent.GetStatValue(SoulStatDefOf.Determination);
            }
        }
        public float TemporaryDetermination
        {
            get
            {
                Hediff overload;
                if (SoulUtilities.IsMonsterForDetermination(pawn))
                    overload = pawn.health.hediffSet.GetFirstHediffOfDef(SoulHediffDefOf.MonsterDeterminationOverload);
                else
                    overload = pawn.health.hediffSet.GetFirstHediffOfDef(SoulHediffDefOf.DeterminationOverload);
                return overload != null ? overload.Severity : 0f;
            }
        }
        public int LV
        {
            get
            {
                if (((Pawn)parent).WorkTagIsDisabled(WorkTags.Violent))
                    return 0;
                int _lv = 1;
                foreach (KeyValuePair<int, int> kvp in LevelUtilities.Custom_LVToEXP)
                {
                    if (EXP >= kvp.Value)
                        _lv = kvp.Key;
                    else
                        break;
                }
                return _lv;
            }
        }
        public int EXP
        {
            get
            {
                return _EXP;
            }
            set
            {
                int change = value - _EXP;
                int oldLV = LV;
                _EXP = value;

                bool leveled = oldLV != LV;

                if (parent.Map == null || parent.DrawPos == null || isbeingloaded)
                    return;
                string sign = change >= 0 ? "+" : "";
                string moteText = leveled ? "DT_LVUpMote".Translate(LV) : sign + "DT_EXPUpMote".Translate(change);
                Color moteColor = Color.white;
                if (leveled)
                {
                    float t = (LV - 1) / 19f; // normalize to 0.0 – 1.0
                    moteColor = Color.Lerp(Color.white, Color.red, t);
                }
                MoteMaker.ThrowText(
                    parent.DrawPos,
                    parent.Map,
                    moteText,
                    moteColor,
                    6f);
            }
        }

        public int NextEXP
        {
            get
            {
                if (LV >= 20)
                    return -1;
                return LevelUtilities.Custom_LVToEXP[LV + 1];
            }
        }

        public void OnPawnGenerated() // Called when the pawn is fully generated
        {
            int setLV = LevelUtilities.GetStartingLV((Pawn)parent);
            EXP = LevelUtilities.Custom_LVToEXP[setLV];
        }

        public override void PostExposeData()
        {
            if (Scribe.mode == LoadSaveMode.LoadingVars)
                isbeingloaded = true;
            else
                isbeingloaded = false;
            Scribe_Defs.Look(ref Variant, "Variant");
            Scribe_Values.Look(ref _EXP, "_EXP");
        }

        public string GetAllModifiers()
        {
            stringBuilder.Length = 0;
            if (Variant.statOffsets != null)
            {
                foreach (StatModifier offset in Variant.statOffsets)
                {
                    stringBuilder.AppendLine(offset.stat.LabelCap + ": " + offset.stat.Worker.ValueToString(offset.value, false, ToStringNumberSense.Offset));
                }
            }
            if (Variant.statFactors != null)
            {
                foreach (StatModifier factor in Variant.statFactors)
                {
                    stringBuilder.AppendLine(factor.stat.LabelCap + ": " + factor.stat.Worker.ValueToString(factor.value, false, ToStringNumberSense.Factor));
                }
            }
            if (Variant.aptitudes != null)
            {
                foreach (Aptitude aptitude in Variant.aptitudes)
                {
                    string sign = (aptitude.level > 0) ? "+" : "";
                    stringBuilder.AppendLine(aptitude.skill.LabelCap + ": " + sign + aptitude.level.ToString());
                }
            }
            return stringBuilder.ToString();
        }

        public string GetLVModifiers()
        {
            stringBuilder.Length = 0;
            if (Variant.statOffsetsPerLV != null)
            {
                foreach (StatModifier offset in Variant.statOffsetsPerLV)
                {
                    if (!Mathf.Approximately(offset.value, 0.0f))
                        stringBuilder.AppendLine(offset.stat.LabelCap + ": " + offset.stat.Worker.ValueToString(offset.value * (LV - 1), false, ToStringNumberSense.Offset));
                }
            }
            return stringBuilder.ToString();
        }

        public override float GetStatOffset(StatDef stat)
        {
            if (Variant != null)
            {
                StatModifier total = new StatModifier();
                total.stat = stat;
                total.value = 0f;

                if (Variant.statOffsets != null)
                {
                    StatModifier soulStat = Variant.statOffsets.Find(s => s.stat == stat);
                    if (soulStat != null)
                    {
                        total.value += soulStat.value;
                    }
                }

                if (Variant.statOffsetsPerLV != null)
                {
                    StatModifier LVStat = Variant.statOffsetsPerLV.Find(s => s.stat == stat);
                    if (LVStat != null && LV > 1)
                    {
                        total.value += LVStat.value * (LV - 1);
                    }
                }
                return total.value;
            }
            return 0.0f;
        }

        public override float GetStatFactor(StatDef stat)
        {
            if (Variant != null && Variant.statFactors != null)
            {
                StatModifier soulStat = Variant.statFactors.Find(s => s.stat == stat);
                if (soulStat != null)
                {
                    return soulStat.value;
                }
            }
            return 1.0f;
        }
        public override void GetStatsExplanation(StatDef stat, StringBuilder sb, string whitespace = "")
        {
            StringBuilder stringBuilder = new StringBuilder();
            float offset = GetStatOffset(stat);
            float factor = GetStatFactor(stat);
            string soulName = Variant.LabelCap;
            if (!Mathf.Approximately(offset, 0.0f))
            {
                stringBuilder.AppendLine((string)(whitespace + "    " + soulName + ": " + stat.Worker.ValueToString(offset, false, ToStringNumberSense.Offset)));
            }
            if (!Mathf.Approximately(factor, 1.0f))
            {
                stringBuilder.AppendLine((string)(whitespace + "    " + soulName + ": " + stat.Worker.ValueToString(factor, false, ToStringNumberSense.Factor)));
            }
            if (stringBuilder.Length == 0)
                return;
            sb.AppendLine((string)(whitespace + "DT_SoulStatModifier".Translate() + ":"));
            sb.Append(stringBuilder.ToString());
        }

        public override void CompTickRare()
        {
            Pawn pawn = parent as Pawn;
            if (!HasSoul)
            {
                if (!pawn.health.hediffSet.HasHediff(SoulHediffDefOf.DeterminationDeficiency))
                {
                    Hediff hediff = pawn.health.AddHediff(SoulHediffDefOf.DeterminationDeficiency);
                    hediff.Severity = 0.001f;
                }    
                return;
            }
            if (pawn.Dead && pawn.Corpse.Age > 7500) // A soul remains in the body for ~3 ingame hours.
            {
                SoulUtilities.MakeSoulless(pawn);
            }
        }
    }
}
