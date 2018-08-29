using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using RimWorld;
using Verse;
using UnityEngine;


namespace SyrTraits
{
    [HarmonyPatch(typeof(InteractionWorker_RomanceAttempt), "RandomSelectionWeight")]
    public static class InteractionWorker_RomanceAttemptPatch
    {
        [HarmonyPriority(Priority.Last)]
        [HarmonyPostfix]
        public static void RandomSelectionWeight_Postfix(ref float __result, Pawn initiator, Pawn recipient)
        {
            __result = RandomSelectionWeight_Method(initiator, recipient);
        }
        private static float RandomSelectionWeight_Method(Pawn initiator, Pawn recipient)
        {
            var comp = recipient.TryGetComp<CompIndividuality>();
            if (LovePartnerRelationUtility.LovePartnerRelationExists(initiator, recipient))
            {
                return 0f;
            }
            float attactiveness = initiator.relations.SecondaryRomanceChanceFactor(recipient);
            if (attactiveness < 0.25f)
            {
                return 0f;
            }
            int opinion = initiator.relations.OpinionOf(recipient);
            if (opinion < 5)
            {
                return 0f;
            }
            if (recipient.relations.OpinionOf(initiator) < 5)
            {
                return 0f;
            }
            float existingLovePartnerFactor = 1f;
            Pawn pawn = LovePartnerRelationUtility.ExistingMostLikedLovePartner(initiator, false);
            if (pawn != null)
            {
                float value = (float)initiator.relations.OpinionOf(pawn);
                existingLovePartnerFactor = Mathf.InverseLerp(50f, -50f, value);
            }
            float romanceFactor = comp.RomanceFactor;
            if (comp == null)
            {
                romanceFactor = 0.5f;
            }
            float attractivenessFactor = Mathf.InverseLerp(0.25f, 1f, attactiveness);
            float opinionFactor = Mathf.InverseLerp(5f, 100f, (float)opinion);
            float genderFactor = 1f;
            if (initiator.gender != recipient.gender && comp != null)
            {
                if (comp.sexuality == CompIndividuality.Sexuality.Straight)
                {
                    genderFactor = 1.0f;
                }
                else if (comp.sexuality == CompIndividuality.Sexuality.Bisexual)
                {
                    genderFactor = 0.75f;
                }
                else if (comp.sexuality == CompIndividuality.Sexuality.Gay)
                {
                    genderFactor = 0.15f;
                }
            }
            if (initiator.gender == recipient.gender && comp != null)
            {
                if (comp.sexuality == CompIndividuality.Sexuality.Gay)
                {
                    genderFactor = 1.0f;
                }
                else if (comp.sexuality == CompIndividuality.Sexuality.Bisexual)
                {
                    genderFactor = 0.75f;
                }
                else if (comp.sexuality == CompIndividuality.Sexuality.Straight)
                {
                    genderFactor = 0.15f;
                }
            }
            return 1.15f * romanceFactor * attractivenessFactor * opinionFactor * existingLovePartnerFactor * genderFactor;
        }
    }
}
