using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Determination
{
    public class ITab_Soul : ITab
    {
        private Vector2 _scrollPosition;
        private Pawn pawn
        {
            get
            {
                if (SelPawn != null)
                    return SelPawn;
                return SelThing is Corpse corpse ? corpse.InnerPawn : null;
            }
        }
        public ITab_Soul()
        {
            size = new Vector2(400f, 430f);
            labelKey = "DT_Soul";
        }

        public override bool IsVisible => pawn.HasComp<CompSoul>() && SoulResearchDefOf.SoulBasics.IsFinished;
        private const int SOUL_SIZE = 64;

        protected override void FillTab()
        {
            CompSoul compSoul = pawn.TryGetComp<CompSoul>();
            if (compSoul == null)
            {
                Text.Font = GameFont.Medium;
                Widgets.Label(new Rect(50, 50, 200f, 100f), "No SoulComp found.");
                return;
            }
            SoulVariantDef variant = compSoul.Variant;
            SoulColorDef color = variant.type;

            string soulHeader = "DT_SoulHeader".Translate(color.LabelCap, variant.LabelCap);
            string soulDesc = variant.description;
            string determination = "DT_Determination".Translate(compSoul.Determination * 100f);
            string modifiers = compSoul.GetAllModifiers();
            string LVModifiersHeader = "DT_LVModifiers".Translate();
            string LVModifiers = compSoul.GetLVModifiers();

            int thisLVEXP = compSoul.EXP - LevelUtilities.Custom_LVToEXP[compSoul.LV];
            int nextLVEXP = compSoul.NextEXP - LevelUtilities.Custom_LVToEXP[compSoul.LV];
            string EXPText = nextLVEXP < 0
                ? "DT_ExpProgress_MaxLV".Translate(compSoul.LV)
                : "DT_ExpProgress".Translate(compSoul.LV, thisLVEXP, nextLVEXP);

            GameFont oldFont = Text.Font;
            TextAnchor oldAnchor = Text.Anchor;
            Text.Anchor = TextAnchor.UpperCenter;

            Text.Font = GameFont.Medium;
            float headerHeight = Text.CalcHeight(soulHeader, 200f);
            float LVModifiersHeaderHeight = Text.CalcHeight(LVModifiersHeader, 200f);

            Text.Font = GameFont.Small;
            float descHeight = Text.CalcHeight(soulDesc, 250f);
            float determinationHeight = Text.CalcHeight(determination, 200f);
            float modifiersHeight = Text.CalcHeight(modifiers, 200f);
            float LVModifiersHeight = Text.CalcHeight(LVModifiers, 200f);
            float EXPTextHeight = Text.CalcHeight(EXPText, 200f);

            const float topPad = 10f;
            const float barHeight = 50f;
            float contentHeight = topPad + SOUL_SIZE + 16f
                + headerHeight + 10f
                + descHeight + 10f
                + determinationHeight
                + modifiersHeight + 30f
                + LVModifiersHeaderHeight + 10f
                + LVModifiersHeight + 30f
                + EXPTextHeight + 10f
                + barHeight + 20f;

            Rect outRect = new Rect(0f, 20f, size.x, size.y - 20f);
            Rect viewRect = new Rect(0f, 0f, outRect.width - 16f, contentHeight);

            Widgets.BeginScrollView(outRect, ref _scrollPosition, viewRect);

            float cx = viewRect.width / 2f;
            float y = topPad;

            Rect soulRect = new Rect(cx - SOUL_SIZE / 2f, y, SOUL_SIZE, SOUL_SIZE);
            Texture2D soulTexture = compSoul.HasSoul ? Determination.soulTextureBig : Determination.soullessTextureBig;
            Color oldcolor = GUI.color;
            GUI.color = color.color;
            if (variant.type.monster)
                GUI.DrawTextureWithTexCoords(soulRect, soulTexture, new Rect(0, 1, 1, -1));
            else
                GUI.DrawTexture(soulRect, soulTexture);
            GUI.color = oldcolor;
            y += SOUL_SIZE + 16f;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(cx - 100f, y, 200f, headerHeight), soulHeader);
            y += headerHeight + 10f;

            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(cx - 125f, y, 250f, determinationHeight), determination);
            y += determinationHeight + 10f;

            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(cx - 125f, y, 250f, descHeight), soulDesc);
            y += descHeight + 10f;

            Widgets.Label(new Rect(cx - 100f, y, 200f, modifiersHeight), modifiers);
            y += modifiersHeight + 30f;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(cx - 100f, y, 200f, LVModifiersHeaderHeight), LVModifiersHeader);
            y += LVModifiersHeaderHeight + 10f;

            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(cx - 100f, y, 200f, LVModifiersHeight), LVModifiers);
            y += LVModifiersHeight + 30f;

            Widgets.Label(new Rect(cx - 100f, y, 200f, EXPTextHeight), EXPText);
            y += EXPTextHeight + 10f;

            float expPercentage = nextLVEXP < 0 ? 1f : (float)thisLVEXP / nextLVEXP;
            Widgets.FillableBar(new Rect(cx - 100f, y, 200f, barHeight), expPercentage);

            Widgets.EndScrollView();

            Text.Font = oldFont;
            Text.Anchor = oldAnchor;
        }
    }
}
