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
    [StaticConstructorOnStartup]
    public static class Determination
    {
        public static bool IsTherapyActive = ModsConfig.IsActive("ferny.ProgressionTherapy");
        public static readonly Texture2D soulIcon = ContentFinder<Texture2D>.Get("UI/soul", true);
        public static readonly Texture2D soulTextureBig = ContentFinder<Texture2D>.Get("Misc/soul", true);
        public static readonly Texture2D soullessTextureBig = ContentFinder<Texture2D>.Get("Misc/soulless", true);
        static Determination()
        {
            // Some explanation:
            // In Rimworld, corpses are an implied def. This means that they aren't defined anywhere in XML, but still exist as defs at runtime.
            // This is an issue when you need to patch them, since there's nothing for the XML to patch.
            // Here I'm taking the blunt approach and simply patching it on startup via C#.
            // If any modders out there are reading this and know of a better way, please let me know.
            foreach (ThingDef pawnDef in DefDatabase<ThingDef>.AllDefsListForReading)
            {
                if (pawnDef.race == null || !pawnDef.race.Humanlike)
                    continue;

                ThingDef corpseDef = pawnDef.race.corpseDef;
                if (corpseDef == null)
                    continue;

                AddTabToDef(corpseDef, typeof(ITab_Soul));
                AddCompToDef(corpseDef, new CompProperties_CompExtractableSoul());
                AddCompToDef(corpseDef, new CompProperties_Disintegrate());

            }
            var harmony = new Harmony("com.dangerousjames.determination");
            harmony.PatchAllUncategorized();
            if (IsTherapyActive)
            {
                var targetType = AccessTools.TypeByName("ProgressionTherapy.StatPart_MentalStability");
                var targetMethod = AccessTools.Method(targetType, "TryGetNeed");
                harmony.Patch(targetMethod, prefix: new HarmonyMethod(typeof(Compatability_Therapy), nameof(Compatability_Therapy.Prefix)));
            }    
                
        }

        private static void AddTabToDef(ThingDef def, System.Type tabType)
        {
            // Keep the Type list in sync (harmless even if unused after PostLoad)
            if (def.inspectorTabs == null)
                def.inspectorTabs = new List<System.Type>();
            if (!def.inspectorTabs.Contains(tabType))
                def.inspectorTabs.Add(tabType);

            // This is the list actually read by ITab-drawing code
            if (def.inspectorTabsResolved == null)
                def.inspectorTabsResolved = new List<InspectTabBase>();

            if (!def.inspectorTabsResolved.Exists(t => t.GetType() == tabType))
            {
                InspectTabBase tabInstance = InspectTabManager.GetSharedInstance(tabType);
                def.inspectorTabsResolved.Add(tabInstance);
            }
        }

        private static void AddCompToDef(ThingDef def, CompProperties comp)
        {
            if (def.comps.Exists(c => c.compClass == comp.compClass))
                return;
            def.comps.Add(comp);
        }
    }
}
