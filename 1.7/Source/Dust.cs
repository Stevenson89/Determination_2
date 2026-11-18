using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace Determination
{
    public class Dust : ThingWithComps,
      IThingHolder,
      IThingHolderTickable
    {
        ThingOwner<Pawn> innerContainer = new();
        public Pawn InnerPawn
        {
            get => innerContainer.Count <= 0 ? null : innerContainer[0];
            set
            {
                if (value == null)
                {
                    innerContainer.Clear();
                }
                else
                {
                    if (innerContainer.Count > 0)
                    {
                        Log.Error("Setting InnerPawn in dust jar that already has one.");
                        innerContainer.Clear();
                    }
                    innerContainer.TryAdd(value, true);
                }
            }
        }

        bool IThingHolderTickable.ShouldTickContents => false;

        public ThingOwner GetDirectlyHeldThings() => innerContainer;
        public override void DynamicDrawPhaseAt(DrawPhase phase, Vector3 drawLoc, bool flip = false)
        {
            if (phase != DrawPhase.Draw)
                return;
            DrawAt(drawLoc, flip);
        }
        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, (IList<Thing>)this.GetDirectlyHeldThings());
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
        }
    }
}
