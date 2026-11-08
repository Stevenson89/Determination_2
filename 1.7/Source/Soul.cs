using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace Determination
{
    public class Soul : IExposable
    {
        public Name Name;
        public BackstoryDef? ChildhoodBackstory;
        public BackstoryDef? AdultBackstory;
        public Ideo? Ideology;
        public float Certainty;
        public Gender Gender;
        public List<SkillRecord> Skills;
        public TraitSet Traits;
        public Faction Faction;
        public Need_Mood? Mood;
        public SoulVariantDef SoulVariant;
        public float Determination;

        public Soul(Pawn pawn)
        {
            Name = pawn.Name;
            ChildhoodBackstory = pawn.story.Childhood;
            AdultBackstory = pawn.story.Adulthood;
            Ideology = pawn.ideo.Ideo;
            Certainty = pawn.ideo.Certainty;
            Gender = pawn.gender;
            Skills = pawn.skills.skills;
            Traits = pawn.story.traits;
            Faction = pawn.Faction;
            Mood = new Need_Mood(pawn);
            CompSoul compSoul = pawn.TryGetComp<CompSoul>();
            SoulVariant = compSoul.Variant;
            Determination = compSoul.Determination;
        }

        public void ExposeData()
        {
            Scribe_Deep.Look(ref Name, "name");
            Scribe_Defs.Look(ref ChildhoodBackstory, "childhoodBackstory");
            Scribe_Defs.Look(ref AdultBackstory, "adultBackstory");
            Scribe_Deep.Look(ref Ideology, "ideology");
            Scribe_Values.Look(ref Certainty, "certainty");
            Scribe_Values.Look(ref Gender, "gender");
            Scribe_Collections.Look(ref Skills, "skills");
            Scribe_Deep.Look(ref Traits, "traits");
            Scribe_Deep.Look(ref Faction, "faction");
            Scribe_Deep.Look(ref Mood, "mood");
        }
    }
}
