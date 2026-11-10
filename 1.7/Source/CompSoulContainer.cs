using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Determination
{
    public class CompSoulContainer : ThingComp
    {
        public Soul? soul;
        public SoulVariantDef? soulVariant => soul?.SoulVariant;
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
                    uniqueUnderlayMat.SetColor("_Color", soulVariant.type.color);
                }
                return uniqueUnderlayMat;
            }
        }

        public bool HasSoul
        {
            get
            {
                return soulVariant != null;
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
                part += soulVariant.description + "\n";
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
            Scribe_Deep.Look(ref soul, "soul");
            Scribe_Values.Look(ref extractAllowed, "extractAllowed");
            Scribe_Values.Look(ref determination, "determination");
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
