using HarmonyLib;
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
        [HarmonyPostfix]
        public static void RandomSelectionWeight_Postfix(ref float __result, Pawn initiator, Pawn recipient)
        {
            if (!SyrIndividuality.RomanceDisabled)
            {
                __result = RandomSelectionWeight_Method(initiator, recipient);
            }
        }
        private static float RandomSelectionWeight_Method(Pawn initiator, Pawn recipient)
        {
            CompIndividuality compOther = recipient.TryGetComp<CompIndividuality>();
            CompIndividuality comp = initiator.TryGetComp<CompIndividuality>();
            if (LovePartnerRelationUtility.LovePartnerRelationExists(initiator, recipient))
            {
                return 0f;
            }
            float attractiveness = initiator.relations.SecondaryRomanceChanceFactor(recipient);
            if (attractiveness < 0.15f)
            {
                return 0f;
            }
            int opinionOfOther = initiator.relations.OpinionOf(recipient);
            if (opinionOfOther < 5)
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
                float opinionOfSpouse = initiator.relations.OpinionOf(pawn);
                existingLovePartnerFactor = Mathf.InverseLerp(50f, -50f, opinionOfSpouse);
            }
            float romanceFactor;
            if (compOther != null)
            {
                romanceFactor = compOther.RomanceFactor * 2f;
            }
            else
            {
                romanceFactor = 1f;
            }
            float attractivenessFactor = Mathf.InverseLerp(0.15f, 1f, attractiveness);
            float opinionFactor = Mathf.InverseLerp(5f, 100f, opinionOfOther);
            float genderFactor = 1f;
            if (initiator.gender != recipient.gender && compOther != null && comp != null)
            {
                if (compOther.sexuality == CompIndividuality.Sexuality.Straight)
                {
                    genderFactor = 1.0f;
                }
                else if (compOther.sexuality == CompIndividuality.Sexuality.Bisexual)
                {
                    genderFactor = 0.75f;
                }
                else if (compOther.sexuality == CompIndividuality.Sexuality.Gay)
                {
                    genderFactor = 0.1f;
                }
                else if (compOther.sexuality == CompIndividuality.Sexuality.Asexual && comp.sexuality == CompIndividuality.Sexuality.Asexual)
                {
                    genderFactor = 1.0f;
                }
            }
            if (initiator.gender == recipient.gender && compOther != null && comp != null)
            {
                if (compOther.sexuality == CompIndividuality.Sexuality.Gay)
                {
                    genderFactor = 1.0f;
                }
                else if (compOther.sexuality == CompIndividuality.Sexuality.Bisexual)
                {
                    genderFactor = 0.75f;
                }
                else if (compOther.sexuality == CompIndividuality.Sexuality.Straight)
                {
                    genderFactor = 0.1f;
                }
                else if (compOther.sexuality == CompIndividuality.Sexuality.Asexual && comp.sexuality == CompIndividuality.Sexuality.Asexual)
                {
                    genderFactor = 0.5f;
                }
            }
            return 1.15f * romanceFactor * attractivenessFactor * opinionFactor * existingLovePartnerFactor * genderFactor;
        }
    }
}
