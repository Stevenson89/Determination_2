using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Determination.Patches
{
    [HarmonyPatch(typeof(CharacterCardUtility), nameof(CharacterCardUtility.DrawCharacterCard))]
    public class Patch_AddSoulIcon
    {
        private static bool ShouldShowIcon(Pawn pawn) =>
            pawn?.RaceProps?.Humanlike == true;
        private static bool ShouldDrawUpsidedown(CompSoul soul) =>
            soul.Variant.type.monster == true;
        public static void Postfix(Rect rect, Pawn pawn)
        {
            Texture2D soulIcon = Determination.soulIcon;
            if (!ShouldShowIcon(pawn))
                return;
            CompSoul soul = pawn.TryGetComp<CompSoul>();
            if (soul == null)
            {
                Log.Error("No compsoul for pawn");
                return;
            }
            SoulVariantDef variant = soul.Variant;
            SoulColorDef color = variant.type;
            Rect iconRect = new Rect(rect.xMax - 160f, rect.y + 3f, 24f, 24f);
            TooltipHandler.TipRegion(iconRect, "DT_SoulIconTip".Translate(color.label, variant.label));
            Color oldColor = GUI.color;
            GUI.color = color.color;
            if (ShouldDrawUpsidedown(soul))
                GUI.DrawTextureWithTexCoords(iconRect, soulIcon, new Rect(0, 1, 1, -1));
            else
                GUI.DrawTexture(iconRect, soulIcon);
            GUI.color = oldColor;
        }
    }
}
