using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace Determination.Migration
{
    public class DeterminationMigrationComponent : GameComponent
    {
        public bool ModFreshlyAdded = true;
        public bool HasModExisted = false;
        public string? LAST_MOD_VERSION;
        // Called when we know this save has had an outdated but modern version of Determination installed
        // When a save is multiple versions behind, this function runs sequentially for each version
        public void Migrate(string oldVersion, string newVersion)
        {

        }

        // Called when we know this save has had determination installed before, but it was a version before version tracking was introduced
        public void MigrateFromLegacy()
        {
            // CompSouls migrate on their own.
            Log.Message("Determination upgraded from legacy version!");
        }

        // Called when we're certain this save has never had a version of Determination installed before
        public void Setup()
        {
            foreach (Pawn pawn in PawnsFinder.AllMapsWorldAndTemporary_Alive)
            {
                CompSoul compSoul = pawn.TryGetComp<CompSoul>();
                if (compSoul != null)
                    SoulUtilities.AssignPawnSoul(pawn, true);
                compSoul?.OnPawnGenerated();
            }
            Log.Message("Determination successfully added to existing save!");
        }
        public DeterminationMigrationComponent(Game game)
        {
            LAST_MOD_VERSION = Determination.CURRENT_MOD_VERSION;
        }
        private static bool VerifyFreshlyAdded()
        {
            foreach (Pawn pawn in PawnsFinder.AllMapsWorldAndTemporary_Alive)
            {
                CompSoul compSoul = pawn.TryGetComp<CompSoul>();
                if (compSoul != null && compSoul.Variant != null && compSoul.Variant.soulless == false)
                    return false;
            }
            return true;
        }
        public override void StartedNewGame()
        {
            base.StartedNewGame();
            ModFreshlyAdded = !HasModExisted;
            HasModExisted = true;
        }
        public override void LoadedGame()
        {
            base.LoadedGame();
            ModFreshlyAdded = !HasModExisted;
            HasModExisted = true;
            bool verifiedFreshlyAdded = false;
            if (ModFreshlyAdded)
                verifiedFreshlyAdded = VerifyFreshlyAdded();
            if (ModFreshlyAdded)
            {
                if (verifiedFreshlyAdded)
                    Setup();
                else
                    MigrateFromLegacy();
            }
            else if (LAST_MOD_VERSION == null)
                Log.Warning("LAST_MOD_VERSION was null. Please report this to the developer!");
            else if (LAST_MOD_VERSION != Determination.CURRENT_MOD_VERSION)
                Migrate(LAST_MOD_VERSION, Determination.CURRENT_MOD_VERSION);
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref HasModExisted, "HasModExisted");
            Scribe_Values.Look(ref LAST_MOD_VERSION, "LAST_MOD_VERSION");
        }
    }
}
