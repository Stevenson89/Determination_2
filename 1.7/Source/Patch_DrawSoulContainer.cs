using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Determination
{
    [HarmonyPatch(typeof(PawnRenderUtility), nameof(PawnRenderUtility.DrawCarriedThing))]
    public class Patch_DrawSoulContainer
    {
        static void Postfix(Pawn pawn, Vector3 drawLoc, Thing carriedThing)
        {
            CompSoulContainer soulContainer = carriedThing.TryGetComp<CompSoulContainer>();
            if (soulContainer == null)
                return;
            if (!soulContainer.HasSoul)
                return;
            Vector3 drawPos = drawLoc;
            drawPos.y -= 0.005f;
            soulContainer.DrawUnderlay(drawPos);
        }
    }
}
