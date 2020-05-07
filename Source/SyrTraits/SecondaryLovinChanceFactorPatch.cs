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
    [HarmonyPatch(typeof(Pawn_RelationsTracker), nameof(Pawn_RelationsTracker.SecondaryLovinChanceFactor))]
    public static class SecondaryLovinChanceFactorPatch
    {
        [HarmonyPostfix]
        public static void SecondaryLovinChanceFactor_Postfix(ref float __result, Pawn ___pawn, Pawn otherPawn, Pawn_RelationsTracker __instance)
        {
            if (!SyrIndividuality.RomanceDisabled)
            {
                CompIndividuality comp = ___pawn.TryGetComp<CompIndividuality>();
                float genderFactor = 1f;
                if (___pawn == otherPawn)
                {
                    __result = 0f;
                }
                if (comp != null && ___pawn.gender != otherPawn.gender)
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
                        genderFactor = 0.1f;
                    }
                    else if (comp.sexuality == CompIndividuality.Sexuality.Asexual)
                    {
                        genderFactor = 0f;
                    }
                }
                else if (comp != null && ___pawn.gender == otherPawn.gender)
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
                        genderFactor = 0.1f;
                    }
                    else if (comp.sexuality == CompIndividuality.Sexuality.Asexual)
                    {
                        genderFactor = 0f;
                    }
                }
                float agePawn = ___pawn.ageTracker.AgeBiologicalYearsFloat;
                float ageOtherPawn = otherPawn.ageTracker.AgeBiologicalYearsFloat;
                float ageFactor = 1f;
                if (___pawn.gender == Gender.Male)
                {
                    float min = agePawn - 30f;
                    float lower = agePawn - 10f;
                    float upper = agePawn + 5f;
                    float max = agePawn + 15f;
                    ageFactor = GenMath.FlatHill(0.2f, min, lower, upper, max, 0.2f, ageOtherPawn);
                }
                else if (___pawn.gender == Gender.Female)
                {
                    float min2 = agePawn - 20f;
                    float lower2 = agePawn - 10f;
                    float upper2 = agePawn + 10f;
                    float max2 = agePawn + 30f;
                    ageFactor = GenMath.FlatHill(0.2f, min2, lower2, upper2, max2, 0.2f, ageOtherPawn);
                }
                float healthFactor = 1f;
                healthFactor *= Mathf.Lerp(0.2f, 1f, otherPawn.health.capacities.GetLevel(PawnCapacityDefOf.Talking));
                float beauty = 0;
                if (otherPawn.RaceProps.Humanlike)
                {
                    beauty = otherPawn.GetStatValue(StatDefOf.PawnBeauty, true);
                }
                float beautyFactor = 1f;
                beautyFactor = beautyCurve.Evaluate(beauty);
                float youthFactorPawn = Mathf.InverseLerp(16f, 18f, agePawn);
                float youthFactorOtherPawn = Mathf.InverseLerp(16f, 18f, ageOtherPawn);
                __result = genderFactor * ageFactor * healthFactor * beautyFactor * youthFactorPawn * youthFactorOtherPawn;
            }
        }

        public static SimpleCurve beautyCurve = new SimpleCurve
        {
            new CurvePoint(-10f, 0.01f),
            new CurvePoint(-2f, 0.3f),
            new CurvePoint(0f, 1f),
            new CurvePoint(1f, 1.8f),
            new CurvePoint(2f, 2.5f),
            new CurvePoint(5f, 3f),
            new CurvePoint(10f, 4f)
        };
    }
}
