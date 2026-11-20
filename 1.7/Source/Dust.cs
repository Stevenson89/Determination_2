using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;

namespace Determination
{
    public class Dust : Corpse
    {
        public override void DynamicDrawPhaseAt(DrawPhase phase, Vector3 drawLoc, bool flip = false)
        {
            DrawAt(drawLoc, flip);
        }

        public override void TickRare()
        {
            base.TickRare();
            TickRareInt();
            if (Destroyed)
                return;
            if (Bugged)
            {
                Log.Error(this?.ToString() + " has null innerPawn. Destroying.");
                Destroy(DestroyMode.Vanish);
            }
        }

        public override void Kill(DamageInfo? dinfo = null, Hediff? exactCulprit = null)
        {
            MethodInfo? grandparentKill = typeof(ThingWithComps).GetMethod(nameof(Kill),BindingFlags.Public | BindingFlags.Instance);
            if (grandparentKill == null)
            {
                throw new NullReferenceException("No grandparent method found in Determination.Dust");
            }
            var handle = grandparentKill.MethodHandle.GetFunctionPointer();
            var itsASuperSuperMethodCalledKill = Activator.CreateInstance(typeof(Action), this, handle);
            if (itsASuperSuperMethodCalledKill != null)
            {
                var youCouldCallItAnUltraKill = (Action)itsASuperSuperMethodCalledKill;
                youCouldCallItAnUltraKill?.Invoke();
            }
        }

        public override string GetInspectString()
        {
            StringBuilder sb = new StringBuilder();
            if (InnerPawn.Faction != null && !InnerPawn.Faction.Hidden)
                sb.AppendLineTagged("Faction".Translate() + ": " + InnerPawn.Faction.NameColored);
            sb.AppendLine((string)"DeadTime".Translate((NamedArgument)Age.ToStringTicksToPeriodVague(vagueMax: false)));
            Hediff_DeathRefusal firstHediff = InnerPawn.health.hediffSet.GetFirstHediff<Hediff_DeathRefusal>();
            if (firstHediff != null && firstHediff.InProgress)
                sb.AppendLine((string)"SelfResurrecting".Translate());
            return sb.ToString().TrimEndNewlines();
        }

        public new void TickRareInt()
        {
            if (AllComps != null)
            {
                int index = 0;
                for (int count = AllComps.Count; index < count; ++index)
                    AllComps[index].CompTickRare();
            }
            if (Destroyed)
                return;
            InnerPawn.TickRare();
        }
    }
}
