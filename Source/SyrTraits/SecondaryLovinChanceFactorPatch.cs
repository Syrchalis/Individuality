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
    [HarmonyPatch(typeof(Pawn_RelationsTracker), nameof(Pawn_RelationsTracker.SecondaryLovinChanceFactor))]
    public static class SecondaryLovinChanceFactorPatch
    {
        [HarmonyPostfix]
        public static void SecondaryLovinChanceFactor_Postfix(ref float __result, Pawn otherPawn, Pawn_RelationsTracker __instance)
        {
            if (!SyrIndividuality.PsychologyIsActive)
            {
                Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
                var comp = pawn.TryGetComp<CompIndividuality>();

                float genderFactor = 1f;
                if (Rand.ValueSeeded(pawn.thingIDNumber ^ 3273711) >= 0.015f)
                {
                    if (pawn.gender != otherPawn.gender && comp != null)
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
                    else if (pawn.gender == otherPawn.gender && comp != null)
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
                }
                float ageBiologicalYearsFloat = pawn.ageTracker.AgeBiologicalYearsFloat;
                float ageBiologicalYearsFloat2 = otherPawn.ageTracker.AgeBiologicalYearsFloat;
                float ageFactor = 1f;
                if (pawn.gender == Gender.Male)
                {
                    if (ageBiologicalYearsFloat2 < 16f)
                    {
                        __result = 0f;
                    }
                    float min = Mathf.Max(16f, ageBiologicalYearsFloat - 30f);
                    float lower = Mathf.Max(20f, ageBiologicalYearsFloat - 10f);
                    ageFactor = GenMath.FlatHill(0.15f, min, lower, ageBiologicalYearsFloat, ageBiologicalYearsFloat + 10f, 0.15f, ageBiologicalYearsFloat2);
                }
                else if (pawn.gender == Gender.Female)
                {
                    if (ageBiologicalYearsFloat2 < 16f)
                    {
                        __result = 0f;
                    }
                    if (ageBiologicalYearsFloat2 < ageBiologicalYearsFloat - 10f)
                    {
                        __result = 0.15f;
                    }
                    if (ageBiologicalYearsFloat2 < ageBiologicalYearsFloat - 3f)
                    {
                        ageFactor = Mathf.InverseLerp(ageBiologicalYearsFloat - 10f, ageBiologicalYearsFloat - 3f, ageBiologicalYearsFloat2) * 0.3f;
                    }
                    else
                    {
                        ageFactor = GenMath.FlatHill(0.3f, ageBiologicalYearsFloat - 3f, ageBiologicalYearsFloat, ageBiologicalYearsFloat + 10f, ageBiologicalYearsFloat + 30f, 0.15f, ageBiologicalYearsFloat2);
                    }
                }
                float healthFactor = 1f;
                healthFactor *= Mathf.Lerp(0.2f, 1f, otherPawn.health.capacities.GetLevel(PawnCapacityDefOf.Talking));
                healthFactor *= Mathf.Lerp(0.2f, 1f, otherPawn.health.capacities.GetLevel(PawnCapacityDefOf.Manipulation));
                healthFactor *= Mathf.Lerp(0.2f, 1f, otherPawn.health.capacities.GetLevel(PawnCapacityDefOf.Moving));
                int beautyDegree = 0;
                if (otherPawn.RaceProps.Humanlike)
                {
                    beautyDegree = otherPawn.story.traits.DegreeOfTrait(TraitDefOf.Beauty);
                }
                float beautyFactor = 1f;
                if (beautyDegree < 0)
                {
                    beautyFactor = 0.5f;
                }
                else if (beautyDegree > 0)
                {
                    beautyFactor = 1.25f;
                }
                else if (beautyDegree > 1)
                {
                    beautyFactor = 2.5f;
                }
                float youthFactorPawn = Mathf.InverseLerp(15f, 18f, ageBiologicalYearsFloat);
                float youthFactorOtherPawn = Mathf.InverseLerp(15f, 18f, ageBiologicalYearsFloat2);
                __result = genderFactor * ageFactor * healthFactor * beautyFactor * youthFactorPawn * youthFactorOtherPawn;
            }
        }
    }
}
