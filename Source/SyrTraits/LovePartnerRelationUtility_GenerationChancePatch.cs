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
    [HarmonyPatch(typeof(LovePartnerRelationUtility), "LovePartnerRelationGenerationChance")]
    public static class LovePartnerRelationUtilityPatch
    {
        [HarmonyPriority(Priority.Last)]
        [HarmonyPostfix]
        public static void LovePartnerRelationGenerationChance_Postfix(ref float __result, Pawn generated, Pawn other, PawnGenerationRequest request, bool ex)
        {
            if (!SyrIndividuality.PsychologyIsActive)
            {
                __result = LovePartnerRelationGenerationChance_Method(generated, other, request, ex);
            }
        }
        private static float LovePartnerRelationGenerationChance_Method(Pawn generated, Pawn other, PawnGenerationRequest request, bool ex)
        {
            if (generated.ageTracker.AgeBiologicalYearsFloat < 14f)
            {
                return 0f;
            }
            if (other.ageTracker.AgeBiologicalYearsFloat < 14f)
            {
                return 0f;
            }
            float LoveChance = 1f;
            float GenderFactor = 1f;
            var comp = other.TryGetComp<CompIndividuality>();
            if (generated.gender != other.gender && comp != null)
            {
                if (comp.sexuality == CompIndividuality.Sexuality.Straight)
                {
                    GenderFactor = 1.0f;
                }
                else if (comp.sexuality == CompIndividuality.Sexuality.Bisexual)
                {
                    GenderFactor = 0.75f;
                }
                else if (comp.sexuality == CompIndividuality.Sexuality.Gay)
                {
                    GenderFactor = 0.05f;
                }
            }
            if (generated.gender == other.gender && comp != null)  
            {
                if (comp.sexuality == CompIndividuality.Sexuality.Gay)
                {
                    GenderFactor = 1.0f;
                }
                else if (comp.sexuality == CompIndividuality.Sexuality.Bisexual)
                {
                    GenderFactor = 0.75f;
                }
                else if (comp.sexuality == CompIndividuality.Sexuality.Straight)
                {
                    GenderFactor = 0.05f;
                }
            }
            if (ex)
            {
                int ExLovers = 0;
                List<DirectPawnRelation> directRelations = other.relations.DirectRelations;
                for (int i = 0; i < directRelations.Count; i++)
                {
                    if (LovePartnerRelationUtility.IsExLovePartnerRelation(directRelations[i].def))
                    {
                        ExLovers++;
                    }
                }
                LoveChance = Mathf.Pow(0.2f, (float)ExLovers);
            }
            else if (LovePartnerRelationUtility.HasAnyLovePartner(other))
            {
                return 0f;
            }
            float generationChanceAgeFactor = Traverse
                .Create(typeof(LovePartnerRelationUtility))
                .Method("GetGenerationChanceAgeFactor", new[] { typeof(Pawn) })
                .GetValue<float>(new object[] { generated });
            float generationChanceAgeFactor2 = Traverse
                .Create(typeof(LovePartnerRelationUtility))
                .Method("GetGenerationChanceAgeFactor", new[] { typeof(Pawn) })
                .GetValue<float>(new object[] { other });
            float generationChanceAgeGapFactor = Traverse
                .Create(typeof(LovePartnerRelationUtility))
                .Method("GetGenerationChanceAgeGapFactor", new[] { typeof(Pawn), typeof(Pawn), typeof(bool) })
                .GetValue<float>(new object[] { generated, other, ex });
            float IncestFactor = 1f;
            if (generated.GetRelations(other).Any((PawnRelationDef x) => x.familyByBloodRelation))
            {
                IncestFactor = 0.01f;
            }
            float MelaninFactor;
            if (request.FixedMelanin != null)
            {
                MelaninFactor = ChildRelationUtility.GetMelaninSimilarityFactor(request.FixedMelanin.Value, other.story.melanin);
            }
            else
            {
                MelaninFactor = PawnSkinColors.GetMelaninCommonalityFactor(other.story.melanin);
            }
            return LoveChance * generationChanceAgeFactor * generationChanceAgeFactor2 * generationChanceAgeGapFactor * GenderFactor * MelaninFactor * IncestFactor;
        }
    }
}
