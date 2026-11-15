using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace Determination
{
    public class QuestNode_Root_WanderingJudge : QuestNode_Root_WandererJoin
    {
        private string signalAccept;
        private string signalReject;
        protected override void RunInt()
        {
            base.RunInt();
            Quest quest = QuestGen.quest;
            quest.Delay(60000, (() => QuestGen_End.End(quest, QuestEndOutcome.Fail)));
        }
        public override Pawn GeneratePawn_NewTemp(Map? map)
        {
            Slate slate = QuestGen.slate;
            PawnGenerationRequest request;
            if (!slate.TryGet("overridePawnGenParams", out request))
            {
                PawnKindDef villager = DefDatabase<PawnKindDef>.GetNamed("WanderingJudge");
                request = new PawnGenerationRequest(villager, tile: map?.Tile, forceGenerateNewPawn: true, colonistRelationChanceFactor: 20f, allowPregnant: false, minChanceToRedressWorldPawn: 0f, fixedBiologicalAge: null, fixedChronologicalAge: null, fixedGender: null, excludeBiologicalAgeRange: null, biologicalAgeRange: null, forceRecruitable: true);
            }
            Pawn pawn = PawnGenerator.GeneratePawn(request);
            pawn.ideo.SetIdeo(Faction.OfPlayer.ideos.PrimaryIdeo);
            if (!pawn.IsWorldPawn())
                Find.WorldPawns.PassToWorld(pawn);
            return pawn;
        }

        public override Pawn GeneratePawn()
        {
            return GeneratePawn_NewTemp(null);
        }

        [Obsolete]
        public override void SendLetter(Quest quest, Pawn pawn)
        {
            SendLetter_NewTemp(quest, pawn, Find.AnyPlayerHomeMap);
        }

        public override void SendLetter_NewTemp(Quest quest, Pawn pawn, Map map)
        {
            TaggedString title = "DT_WanderingJudgeLetterTitle".Translate(pawn.Named("PAWN")).AdjustedFor(pawn);
            TaggedString text = "DT_WanderingJudgeLetterBody".Translate(pawn.Named("PAWN")).AdjustedFor(pawn);
            QuestNode_Root_WandererJoin_WalkIn.AppendCharityInfoToLetter("JoinerCharityInfo".Translate((NamedArgument)(Thing)pawn), ref text);
            PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref text, ref title, pawn);
            if (pawn.DevelopmentalStage.Juvenile())
            {
                string stringTicksToPeriod = (pawn.ageTracker.AgeBiologicalYears * 3600000).ToStringTicksToPeriod();
                text += "\n\n" + "RefugeePodCrash_Child".Translate(pawn.Named("PAWN"), stringTicksToPeriod.Named("AGE"));
            }
            QuestNode_Root_WandererJoin_WalkIn.ApplyBestSkillInfoToLetter(ref text, pawn);
            ChoiceLetter_AcceptJoiner let = (ChoiceLetter_AcceptJoiner)LetterMaker.MakeLetter(title, text, LetterDefOf.AcceptJoiner);
            let.signalAccept = signalAccept;
            let.signalReject = signalReject;
            let.quest = quest;
            let.overrideMap = map;
            let.StartTimeout(60000);
            Find.LetterStack.ReceiveLetter((Letter)let);
        }

        protected override void AddSpawnPawnQuestParts(Quest quest, Map map, Pawn pawn)
        {
            signalAccept = QuestGenUtility.HardcodedSignalWithQuestID("Accept");
            signalReject = QuestGenUtility.HardcodedSignalWithQuestID("Reject");
            quest.Signal(signalAccept, (() =>
            {
                quest.SetFaction((IEnumerable<Thing>)Gen.YieldSingle(pawn), Faction.OfPlayer);
                quest.PawnsArrive(Gen.YieldSingle(pawn), mapParent: map.Parent);
                QuestGen_End.End(quest, QuestEndOutcome.Success);
            }));
            quest.Signal(signalReject, (() =>
            {
                quest.GiveDiedOrDownedThoughts(pawn, PawnDiedOrDownedThoughtsKind.DeniedJoining);
                QuestGen_End.End(quest, QuestEndOutcome.Fail);
            }));
        }
    }
}
