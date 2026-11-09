using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;
using Waylith.Math;

namespace Determination
{
    public class Soul : IExposable
    {
        public Pawn? OriginalPawn;
        public Name? Name;
        public string? Title;
        public BackstoryDef? ChildhoodBackstory;
        public BackstoryDef? AdultBackstory;
        public Ideo? Ideology;
        public float Certainty;
        public Gender Gender;
        public List<SkillRecord>? Skills;
        public List<Trait> Traits;
        public Faction? Faction;
        public Dictionary<Pawn, int> Opinions;
        public SoulVariantDef SoulVariant;
        public float Determination;
        public bool Psychopath;

        List<Pawn> keysList = [];
        List<int> valuesList = [];

        public Soul(Pawn pawn)
        {
            OriginalPawn = pawn;
            Name = pawn.Name;
            Title = pawn.story.Title;
            ChildhoodBackstory = pawn.story.Childhood;
            AdultBackstory = pawn.story.Adulthood;
            Ideology = pawn.ideo.Ideo;
            Certainty = pawn.ideo.Certainty;
            Gender = pawn.gender;
            Skills = pawn.skills.skills;
            Traits = [];
            foreach (Trait trait in pawn.story.traits.allTraits)
                Traits.Add(new Trait(trait.def, trait.Degree));
            Faction = pawn.Faction;
            Opinions = [];
            foreach (Pawn pawn1 in PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive)
            {
                Opinions.Add(pawn1, GetDeadOpinion(pawn, pawn1));
            }
            CompSoul compSoul = pawn.TryGetComp<CompSoul>();
            SoulVariant = compSoul.Variant;
            Determination = compSoul.Determination;
            Psychopath = pawn.story.traits.HasTrait(TraitDefOf.Psychopath);
        }

        private int GetDeadOpinion(Pawn pawn, Pawn other)
        {
            if (!other.RaceProps.Humanlike || pawn == other)
            {
                return 0;
            }

            int num = 0;
            foreach (PawnRelationDef relation in pawn.GetRelations(other))
            {
                num += relation.opinionOffset;
            }

            if (pawn.RaceProps.Humanlike && pawn.needs?.mood != null)
            {
                num += pawn.needs.mood.thoughts.TotalOpinionOffset(other);
            }

            if (num != 0)
            {
                float num2 = 1f;
                List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
                for (int i = 0; i < hediffs.Count; i++)
                {
                    if (hediffs[i].CurStage != null)
                    {
                        num2 *= hediffs[i].CurStage.opinionOfOthersFactor;
                    }
                }

                num = Mathf.RoundToInt((float)num * num2);
            }

            if (num > 0 && pawn.HostileTo(other))
            {
                num = 0;
            }

            return Mathf.Clamp(num, -100, 100);
        }

        public Soul()
        {
            OriginalPawn = null;
            Name = null;
            Title = "";
            ChildhoodBackstory = null;
            AdultBackstory = null;
            Ideology = null;
            Certainty = 1f;
            Gender = Gender.None;
            Skills = null;
            Traits = [];
            Faction = null;
            Opinions = [];
            SoulVariant = SoulVariantDefOf.SoullessHuman;
            Determination = 0f;
            Psychopath = false;
        }

        public void ExposeData()
        {
            Scribe_References.Look(ref OriginalPawn, "OriginalPawn", saveDestroyedThings: true);
            Scribe_Deep.Look(ref Name, "Name");
            Scribe_Values.Look(ref Title, "Title");
            Scribe_Defs.Look(ref ChildhoodBackstory, "ChildhoodBackstory");
            Scribe_Defs.Look(ref AdultBackstory, "AdultBackstory");
            Scribe_References.Look(ref Ideology, "Ideology");
            Scribe_Values.Look(ref Certainty, "Certainty");
            Scribe_Values.Look(ref Gender, "Gender");
            Scribe_Collections.Look(ref Skills, "Skills");
            Scribe_Collections.Look(ref Traits, "Traits");
            Scribe_References.Look(ref Faction, "Faction");
            Scribe_Collections.Look(ref Opinions, "Opinions", LookMode.Reference, LookMode.Value, ref keysList, ref valuesList, saveDestroyedKeys: true, saveDestroyedValues: true);
            Scribe_Defs.Look(ref SoulVariant, "SoulVariant");
            Scribe_Values.Look(ref Determination, "Determination");
            Scribe_Values.Look(ref Psychopath, "Psychopath");
        }
        
        public enum Loyalty
        {
            LOYAL,
            DISLOYAL
        }

        public Loyalty GetLoyaltyTo(Pawn other)
        {
            if (Traits.ContainsAny(t => t.def == TraitDefOf.Psychopath))
            {
                return Loyalty.DISLOYAL;
            }
            if (Traits.ContainsAny(t => t.def == TraitDefOf.Kind))
            {
                return Loyalty.LOYAL;
            }
            int? opinion = Opinions[other];
            if (opinion == null)
                return Loyalty.LOYAL;
            else return opinion.Value > -10 ? Loyalty.LOYAL : Loyalty.DISLOYAL;
        }
    }
}
