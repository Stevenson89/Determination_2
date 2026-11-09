using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace Determination
{
    public class ITab_AdvancedSoul : ITab
    {
        private Vector2 _scrollPosition;
        private StringBuilder traitsSB = new StringBuilder();
        private Soul? soul
        {
            get
            {
                return SelThing.TryGetComp<CompSoulContainer>()?.soul;
            }
        }
        public ITab_AdvancedSoul()
        {
            size = new Vector2(400f, 430f);
            labelKey = "DT_Soul";
        }

        public override bool IsVisible => soul != null;
        private const int SOUL_SIZE = 64;

        protected override void FillTab()
        {
            if (soul == null)
            {
                Text.Font = GameFont.Medium;
                Widgets.Label(new Rect(50, 50, 200f, 100f), "No soul is connected.");
                return;
            }
            SoulVariantDef variant = soul.SoulVariant;
            SoulColorDef color = variant.type;
            const float DEFAULT_SPACING = 2f;

            GameFont oldFont = Text.Font;
            TextAnchor oldAnchor = Text.Anchor;
            Text.Anchor = TextAnchor.UpperCenter;

            Text.Font = GameFont.Medium;
            string nameText = soul.Name.ToStringFull;
            float nameHeight = Text.CalcHeight(nameText, 300);

            string? titleText = soul.Title?.CapitalizeFirst();
            float titleHeight = titleText != null ? Text.CalcHeight(titleText, 300) : 0;

            string soulText = "DT_SoulIconTip".Translate(color.LabelCap, variant.LabelCap);
            float soulHeight = Text.CalcHeight(soulText, 300);

            Text.Font = GameFont.Small;
            string naturalDeterminationText = "DT_NaturalDetermination".Translate(soul.Determination * 100f);
            float naturalDeterminationHeight = Text.CalcHeight(naturalDeterminationText, 300);

            string genderText = "DT_Gender".Translate(GenderUtility.GetLabel(soul.Gender));
            float genderHeight = Text.CalcHeight(genderText, 300);
            traitsSB.Clear();
            traitsSB.Append("DT_TraitList".Translate().RawText);
            traitsSB.AppendLine();
            traitsSB.AppendLine();
            foreach (Trait trait in soul.Traits)
            {
                traitsSB.AppendLine(trait.LabelCap);
            }
            string traitsText = traitsSB.ToString();
            float traitsHeight = Text.CalcHeight(traitsText, 300);

            string? factionText = soul.Faction != null ? "DT_Faction".Translate(soul.Faction.Name) : null;
            float factionHeight = factionText != null ? Text.CalcHeight(factionText, 300) : 0;

            string? ideologyText = soul.Ideology != null ? "DT_Ideology".Translate(soul.Ideology.name) : null;
            float ideologyHeight = ideologyText != null ? Text.CalcHeight(ideologyText, 300) : 0;

            const float topPad = 10f;
            float contentHeight = topPad + SOUL_SIZE + 16f
                + nameHeight + 5f
                + titleHeight + DEFAULT_SPACING
                + soulHeight + DEFAULT_SPACING
                + naturalDeterminationHeight + DEFAULT_SPACING
                + genderHeight + DEFAULT_SPACING
                + traitsHeight + DEFAULT_SPACING
                + factionHeight + DEFAULT_SPACING
                + ideologyHeight + DEFAULT_SPACING;

            Rect outRect = new Rect(0f, 20f, size.x, size.y - 20f);
            Rect viewRect = new Rect(0f, 0f, outRect.width - 16f, contentHeight);

            float cx = viewRect.width / 2f;
            float y = topPad;


            // DRAWING STARTS HERE
            Widgets.BeginScrollView(outRect, ref _scrollPosition, viewRect);
            Rect soulRect = new Rect(cx - SOUL_SIZE / 2f, y, SOUL_SIZE, SOUL_SIZE);
            Texture2D soulTexture = Determination.soulTextureBig;
            Color oldcolor = GUI.color;
            GUI.color = color.color;
            GUI.DrawTexture(soulRect, soulTexture);
            GUI.color = oldcolor;
            y += SOUL_SIZE + 16f;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(cx - 150f, y, 300f, nameHeight), nameText);
            y += nameHeight + 5f;

            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(cx - 150f, y, 300f, titleHeight), titleText);
            y += titleHeight + DEFAULT_SPACING;

            Widgets.Label(new Rect(cx - 150f, y, 300f, soulHeight), soulText);
            y += soulHeight + DEFAULT_SPACING;

            Widgets.Label(new Rect(cx - 150f, y, 300f, naturalDeterminationHeight), naturalDeterminationText);
            y += naturalDeterminationHeight + DEFAULT_SPACING;

            Widgets.Label(new Rect(cx - 150f, y, 300f, genderHeight), genderText);
            y += genderHeight + DEFAULT_SPACING;

            Widgets.Label(new Rect(cx - 150f, y, 300f, traitsHeight), traitsText);
            y += traitsHeight + DEFAULT_SPACING;

            if (factionText != null)
            {
                Widgets.Label(new Rect(cx - 150f, y, 300f, factionHeight), factionText);
                y += factionHeight + DEFAULT_SPACING;
            }

            if (ideologyText != null)
            {
                Widgets.Label(new Rect(cx - 150f, y, 300f, ideologyHeight), ideologyText);
                y += ideologyHeight + DEFAULT_SPACING;
            }

            Widgets.EndScrollView();
            Text.Font = oldFont;
            Text.Anchor = oldAnchor;
        }
    }
}
