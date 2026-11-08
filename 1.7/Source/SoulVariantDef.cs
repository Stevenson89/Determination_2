using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Determination
{
    public class SoulVariantDef : Def
    {
        public SoulColorDef type;
        public List<StatModifier> statOffsets;
        public List<StatModifier> statFactors;
        public List<Aptitude> aptitudes;
        public List<StatModifier> statOffsetsPerLV;
        public int KillThirstAtLV;
        public bool soulless;

        public int AptitudeFor(SkillDef skill)
        {
            int num = 0;
            if (aptitudes.NullOrEmpty<Aptitude>())
                return num;
            for (int index = 0; index < aptitudes.Count; ++index)
            {
                if (aptitudes[index].skill == skill)
                    num += aptitudes[index].level;
            }
            return num;
        }
    }
}
