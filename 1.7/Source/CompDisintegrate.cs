using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Determination
{
    public class CompDisintegrate : ThingComp
    {
        public float timer = 2;
        private bool active = false;
        public void Activate()
        {
            timer = SoulUtilities.IsBossMonster((parent as Corpse).InnerPawn) ? 6 : 0;
            active = true;
        }
        public override void CompTickRare()
        {
            if (active)
            {
                timer -= 1;
                FleckMaker.ThrowDustPuff(parent.DrawPos, parent.Map, 1f);
                FleckMaker.ThrowDustPuff(parent.DrawPos, parent.Map, 1f);
                if (timer <= 0)
                {
                    Corpse corpse = parent as Corpse;
                    corpse.Strip(false);
                    corpse.Kill();
                }
            }
        }
        public override string CompInspectStringExtra()
        {
            if (active)
                return "DT_Disintegrating".Translate();
            else
                return null;
        }
    }
}
