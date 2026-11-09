using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

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
        public SoulVariantDef SoulVariant;
        public float Determination;
        public bool Psychopath;

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
            CompSoul compSoul = pawn.TryGetComp<CompSoul>();
            SoulVariant = compSoul.Variant;
            Determination = compSoul.Determination;
            Psychopath = pawn.story.traits.HasTrait(TraitDefOf.Psychopath);
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
            Scribe_Defs.Look(ref SoulVariant, "SoulVariant");
            Scribe_Values.Look(ref Determination, "Determination");
            Scribe_Values.Look(ref Psychopath, "Psychopath");
        }
        
        public enum Loyalty
        {
            HOST,
            LOYAL,
            DISLOYAL
        }

        public Loyalty GetLoyaltyTo(Pawn pawn)
        {
            return Loyalty.LOYAL;
        }
    }
}
