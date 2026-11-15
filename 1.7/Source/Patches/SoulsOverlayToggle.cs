using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace Determination.Patches
{
    [HarmonyPatch(typeof(PlaySettings), "DoMapControls")]
    public static class SoulsOverlayToggle
    {
        static void Postfix(WidgetRow row)
        {
            row.ToggleableIcon(ref Determination.ShowSoulOverlay, Determination.soulIcon, "DT_ShowSoulOverlay".Translate());
        }
    }
}
