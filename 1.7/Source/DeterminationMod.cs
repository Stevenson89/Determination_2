using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Determination
{
    public class DeterminationMod : Mod
    {
        DeterminationSettings settings;
        public DeterminationMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<DeterminationSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);
            //listing.CheckboxLabeled("DT_AutoAssignMonsters".Translate(), ref settings.AutoAssignMonsters);
            listing.CheckboxLabeled("DT_FatalDeterminationDeficiency".Translate(), ref settings.FatalDeterminationDeficiency);
            listing.Label("DT_KillingIntentFactor".Translate(settings.KillingIntentFactor.ToString("F1")));
            settings.KillingIntentFactor = Mathf.Round(listing.Slider(settings.KillingIntentFactor, 0f, 5f) / 0.1f) * 0.1f;
            listing.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "DT_DeterminationModName".Translate();
        }
    }
}
