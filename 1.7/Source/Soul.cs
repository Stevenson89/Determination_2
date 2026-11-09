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
        public Need_Mood? Mood;
        public SoulVariantDef SoulVariant;
        public float Determination;

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
            Mood = new Need_Mood(pawn);
            CompSoul compSoul = pawn.TryGetComp<CompSoul>();
            SoulVariant = compSoul.Variant;
            Determination = compSoul.Determination;
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
            Mood = null;
            SoulVariant = SoulVariantDefOf.SoullessHuman;
            Determination = 0f;
        }

        public void ExposeData()
        {
            Scribe_References.Look(ref OriginalPawn, "OriginalPawn");
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
            Scribe_Deep.Look(ref Mood, "Mood", OriginalPawn);
            Scribe_Defs.Look(ref SoulVariant, "SoulVariant");
            Scribe_Values.Look(ref Determination, "Determination");
        }
    }
}
