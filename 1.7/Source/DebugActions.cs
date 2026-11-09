using LudeonTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Determination
{
    public static class DebugActions
    {
        [DebugAction("Determination", null, false, false, false, false, false, 0, false, actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void MakeMaxLV(Pawn pawn)
        {
            pawn.TryGetComp<CompSoul>().EXP = LevelUtilities.Custom_LVToEXP[20];
        }
        [DebugAction("Determination", null, false, false, false, false, false, 0, false, actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void AddSoulAbsorbed(Pawn pawn)
        {
            SoulUtilities.IncreasePawnSouls(pawn);
        }
        [DebugAction("Determination", null, false, false, false, false, false, 0, false, actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void StartSoulRebellion(Pawn pawn)
        {
            pawn.TryGetComp<CompSoul>()?.StartRebellion();
        }
        [DebugAction("Determination", allowedGameStates = AllowedGameStates.PlayingOnMap, actionType = DebugActionType.ToolMapForPawns)]
        public static void SetSoulType(Pawn pawn)
        {
            List<DebugMenuOption> options = new List<DebugMenuOption>();
            foreach (SoulVariantDef soulType in DefDatabase<SoulVariantDef>.AllDefsListForReading)
            {
                options.Add(new DebugMenuOption(soulType.LabelCap, DebugMenuOptionMode.Action, () =>
                {
                    CompSoul soul = pawn.TryGetComp<CompSoul>();
                    if (soul != null)
                    {
                        soul.Variant = soulType;
                    }
                    else
                        Log.Warning("Selected pawn has no CompSoul! (This is usually because the selected pawn was not a humanlike)");
                }));
            }
            Find.WindowStack.Add(new Dialog_DebugOptionListLister(options));
        }
    }
}
