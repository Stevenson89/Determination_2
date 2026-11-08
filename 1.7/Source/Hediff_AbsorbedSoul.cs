using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using RimWorld;

namespace Determination
{
    public class Hediff_AbsorbedSoul : Hediff
    {
        private const int MaxDefinedLevel = 7;
        private int cachedLevel = -1;
        private HediffStage cachedStage;
        public int Level => Mathf.Max(1, Mathf.FloorToInt(Severity));
        HediffStage FinalStage => def.stages[MaxDefinedLevel - 1];
        public override string LabelInBrackets
        {
            get
            {
                return Level.ToString();
            }
        }

        public override HediffStage CurStage
        {
            get
            {
                if (Level <= MaxDefinedLevel)
                {
                    int index = Mathf.Min(Level, def.stages.Count) - 1;
                    return def.stages[index];
                }
                if (Level != cachedLevel)
                {
                    cachedStage = MakeStage(Level);
                    cachedLevel = Level;
                }
                return cachedStage;
            }
        }

        public override int CurStageIndex =>
            Mathf.Min(Level, def.stages.Count) - 1;

        private HediffStage MakeStage(int level)
        {
            int extraLevels = level - MaxDefinedLevel;

            HediffStage thisStage = new HediffStage
            {
                label = "error" + level.ToString(),
                disablesNeeds = FinalStage.disablesNeeds,
                regeneration = FinalStage.regeneration,
                painFactor = FinalStage.painFactor,
                totalBleedFactor = FinalStage.totalBleedFactor,
                capMods = new List<PawnCapacityModifier>(),
                statOffsets = new List<StatModifier>(),
                statFactors = new List<StatModifier>()
            };

            thisStage.regeneration = DiminishingBonus(FinalStage.regeneration, extraLevels);

            foreach (StatModifier baseFactor in FinalStage.statFactors)
            {
                if (baseFactor.value > 1)
                {
                    thisStage.statOffsets.Add(new StatModifier
                    {
                        stat = baseFactor.stat,
                        value = DiminishingBonus(baseFactor.value, extraLevels)
                    });
                }
            }

            foreach (PawnCapacityModifier baseModifier in FinalStage.capMods)
            {
                thisStage.capMods.Add(new PawnCapacityModifier
                {
                    capacity = baseModifier.capacity,
                    offset = DiminishingBonus(baseModifier.offset, extraLevels)
                });
            }

            thisStage.overrideTooltip = FinalStage.overrideTooltip;

            return thisStage;
        }

        private float DiminishingBonus(float baseValue, int extraLevels)
        {
            float k = baseValue * 0.15f;
            return baseValue + k * Mathf.Log(1 + extraLevels);
        }
    }
}
