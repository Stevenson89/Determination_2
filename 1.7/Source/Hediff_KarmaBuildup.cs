using System;
using System.Collections.Generic;
using System.Text;
using Verse;
using Waylith.Math;

namespace Determination
{
    public class Hediff_KarmaBuildup : HediffWithComps
    {
        public static DamageDef DamageDef = DefDatabase<DamageDef>.GetNamed("Karma");
        public override string LabelInBrackets => Mathf.Ceil(Severity).ToString();
        int TicksSinceKarma = 0;
        int TicksBetweenKarma // These values are twice that of Undertales because Undertale runs at 30 FPS
        {
            get
            {
                if (Severity >= 40f)
                    return 2;
                else if (Severity >= 30)
                    return 4;
                else if (Severity >= 20)
                    return 10;
                else if (Severity >= 10)
                    return 30;
                else
                    return 60;
            }
        }
        private void ApplyKarmaBurn()
        {
            pawn.TakeDamage(new DamageInfo(DamageDef, 1));
        }
        public override void PostTick()
        {
            base.PostTick();
            TicksSinceKarma += 1;
            if (TicksSinceKarma >= TicksBetweenKarma)
            {
                ApplyKarmaBurn();
                Severity -= 1;
                TicksSinceKarma = 0;
            }
        }
    }
}
