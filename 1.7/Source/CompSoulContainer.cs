using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;
using Verse;

namespace Determination
{
    public class CompSoulContainer : ThingComp
    {
        public Soul? soul;
        public SoulVariantDef? soulVariant; // This is never set normally. It is only used to migrate 1.0 versions of the mod to 2.0
        public SoulVariantDef? SoulVariant => soul?.SoulVariant;
        public bool extractAllowed = false;
        public bool CanExtract => soul != null && determination >= 1f;
        public float determination = 1.0f;

        private Material? uniqueUnderlayMat;

        public Material UniqueUnderlayMat
        {
            get
            {
                if (uniqueUnderlayMat == null)
                {
                    Material baseMat = MaterialPool.MatFrom("Misc/soul", ShaderDatabase.Transparent);
                    uniqueUnderlayMat = new Material(baseMat);
                    uniqueUnderlayMat.SetColor("_Color", SoulVariant.type.color);
                }
                return uniqueUnderlayMat;
            }
        }

        public bool HasSoul
        {
            get
            {
                return SoulVariant != null;
            }
        }

        public void DrawUnderlay(Vector3 pos)
        {
            Matrix4x4 matrix = default;
            matrix.SetTRS(pos, Quaternion.identity, new Vector3(parent.def.size.x / 3f, 1f, parent.def.size.z / 3f));

            Graphics.DrawMesh(MeshPool.plane10, matrix, UniqueUnderlayMat, 0);
        }

        public override void PostDraw()
        {
            base.PostDraw();
            if (HasSoul && parent.Spawned)
            {
                Vector3 drawPos = parent.DrawPos;
                drawPos.y -= 0.005f; // lower altitude = drawn beneath the parent's own graphic
                DrawUnderlay(drawPos);
            }
        }

        public override string CompInspectStringExtra()
        {
            if (HasSoul)
            {
                return "DT_Determination".Translate(determination * 100f);
            }
            return "";
        }

        public override string GetDescriptionPart()
        {
            string part = "";
            if (HasSoul)
            {
                part += SoulVariant.description + "\n";
                part += "DT_Determination".Translate(determination * 100f);
            }
            else
            {
                part += "DT_EmptySoulContainer".Translate() + "\n";
            }
            return part;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            bool legacySoul = false;
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                XmlNode? node = Scribe.loader.curXmlParent["soul"];
                if (node != null)
                {
                    if (DefDatabase<SoulVariantDef>.GetNamedSilentFail(node.InnerText) != null)
                        legacySoul = true;
                }
            }
            Log.Message($"Legacy soul : {(legacySoul ? "True" : "False")}");
            if (legacySoul)
            {
                Scribe_Defs.Look(ref soulVariant, "soul");
                Log.Message(soulVariant?.ToString());
            }
            else
                Scribe_Deep.Look(ref soul, "soul");
            Scribe_Values.Look(ref extractAllowed, "extractAllowed");
            Scribe_Values.Look(ref determination, "determination");
            if (Scribe.mode == LoadSaveMode.LoadingVars && soulVariant != null && soul == null)
            {
                Log.Message("Making legacy soul");
                soul = new Soul(soulVariant);
                soulVariant = null;
            }
        }

        public override void CompTickRare()
        {
            if (HasSoul)
            {
                determination = Mathf.Clamp(determination + 0.005f, 0f, 1f);
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if ((SoulResearchDefOf.DeterminationExtraction.IsFinished && soul != null) || extractAllowed)
            {
                yield return new Command_Toggle
                {
                    defaultLabel = "Extract",
                    defaultDesc = "Toggle automatic extraction of determination from this soul",
                    icon = ContentFinder<Texture2D>.Get("Things/Item/DeterminationSyringe"),
                    activateIfAmbiguous = false,
                    isActive = () => extractAllowed,
                    toggleAction = () =>
                    {
                        extractAllowed = !extractAllowed;
                    }
                };
            }
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            if (uniqueUnderlayMat != null)
                UnityEngine.Object.Destroy(uniqueUnderlayMat);
        }
    }
}
