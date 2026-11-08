using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using static UnityEngine.GraphicsBuffer;

namespace Determination
{
    public class FloatMenuOptionProvider_AbsorbSoul : FloatMenuOptionProvider
    {
        protected override bool Drafted => true;

        protected override bool Undrafted => true;

        protected override bool Multiselect => false;

        protected override FloatMenuOption GetSingleOptionFor(Thing clickedThing, FloatMenuContext context)
        {
            if (clickedThing is Corpse corpse)
            {
                if (!SoulUtilities.CanAbsorbTargetSoul(context.FirstSelectedPawn, corpse.InnerPawn))
                {
                    return null;
                }
                return new FloatMenuOption("DT_AbsorbSoul".Translate(), () =>
                {
                    corpse.SetForbidden(false);
                    Pawn pawn = context.FirstSelectedPawn;
                    Job job = new Job(DefDatabase<JobDef>.GetNamed("AbsorbSoul"), corpse);
                    pawn.jobs.StartJob(job, JobCondition.InterruptForced);
                });
            }
            else
            {
                CompSoulContainer soulContainer = clickedThing.TryGetComp<CompSoulContainer>();
                if (soulContainer == null)
                {
                    return null;
                }
                if (soulContainer.soulVariant == null)
                    return null;
                if (!SoulUtilities.CanAbsorbSoul(context.FirstSelectedPawn, soulContainer.soulVariant))
                {
                    return null;
                }
                return new FloatMenuOption("DT_AbsorbSoul".Translate(), () =>
                {
                    Pawn pawn = context.FirstSelectedPawn;
                    Job job = new Job(DefDatabase<JobDef>.GetNamed("AbsorbSoulFromContainer"), clickedThing);
                    pawn.jobs.StartJob(job, JobCondition.InterruptForced);
                });
            }
        }
    }
}
