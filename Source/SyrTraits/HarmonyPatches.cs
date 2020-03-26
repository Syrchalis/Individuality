using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using RimWorld;
using Verse;
using UnityEngine;
using Verse.AI;
using RimWorld.Planet;
using System.Reflection.Emit;

namespace SyrTraits
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        public static IEnumerable<ThingDef> beautyPlants;
        static HarmonyPatches()
        {
            var harmony = new Harmony("Syrchalis.Rimworld.Traits");
            harmony.Patch(typeof(JobDriver_ConstructFinishFrame).
                GetNestedType("<>c__DisplayClass4_0", BindingFlags.NonPublic | BindingFlags.Instance).
                GetMethod("<MakeNewToils>b__1", BindingFlags.NonPublic | BindingFlags.Instance),
                transpiler: new HarmonyMethod(typeof(JobDriver_ConstructFinishFramePatch).GetMethod(nameof(JobDriver_ConstructFinishFramePatch.JobDriver_ConstructFinishFrame_Transpiler))));
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            beautyPlants = DefDatabase<ThingDef>.AllDefs.Where((ThingDef x) => x?.plant != null && x.plant.purpose == PlantPurpose.Beauty);
        }
    }


    //Green Thumb Trait
    [HarmonyPatch(typeof(WorkGiver_GrowerSow), nameof(WorkGiver_GrowerSow.JobOnCell))]
    public static class WorkGiver_GrowerSowPatch
    {
        public static bool greenThumb = false;
        [HarmonyPrefix]
        public static void WorkGiver_GrowerSow_Prefix()
        {
            greenThumb = true;
        }

        [HarmonyFinalizer]
        public static void WorkGiver_GrowerSow_Postfix()
        {
            greenThumb = false;
        }
    }

    [HarmonyPatch(typeof(Command_SetPlantToGrow), "WarnAsAppropriate")]
    public static class WarnAsAppropiatePatch
    {
        [HarmonyPrefix]
        public static void WarnAsAppropiate_Prefix()
        {
            WorkGiver_GrowerSowPatch.greenThumb = true;
        }

        [HarmonyFinalizer]
        public static void WarnAsAppropiate_Postfix()
        {
            WorkGiver_GrowerSowPatch.greenThumb = false;
        }
    }

    [HarmonyPatch(typeof(Pawn_SkillTracker), nameof(Pawn_SkillTracker.GetSkill))]
    public static class GetSkillPatch
    {
        [HarmonyPostfix]
        public static void GetSkill_Postfix(ref SkillRecord __result, Pawn_SkillTracker __instance, SkillDef skillDef, Pawn ___pawn)
        {
            if (___pawn.story.traits.HasTrait(SyrTraitDefOf.SYR_GreenThumb) && WorkGiver_GrowerSowPatch.greenThumb)
            {
                SkillRecord fakeRecord = new SkillRecord(___pawn, SkillDefOf.Plants);
                fakeRecord.Level = 20;
                __result = fakeRecord;
            }
        }
    }



    //Keen Eye Trait
    [HarmonyPatch(typeof(Mineable), "TrySpawnYield")]
    public static class TrySpawnYieldPatch
    {
        [HarmonyPostfix]
        public static void TrySpawnYield_Postfix(Mineable __instance, Map map, float yieldChance, bool moteOnWaste, Pawn pawn)
        {
            if (pawn?.story?.traits != null && pawn.story.traits.HasTrait(SyrTraitDefOf.SYR_KeenEye) && Rand.Value < FactorFromHilliness(map.TileInfo.hilliness))
            {
                ThingDef thingDef = DefDatabase<ThingDef>.AllDefs.RandomElementByWeightWithFallback(delegate (ThingDef d)
                {
                    if (d.building == null)
                    {
                        return 0f;
                    }
                    if (d == ThingDefOf.MineableComponentsIndustrial)
                    {
                        return d.building.mineableScatterCommonality * 0.4f;
                    }
                    return d.building.mineableScatterCommonality;
                }, null);
                int num = Mathf.Max(1, Mathf.RoundToInt(thingDef.building.mineableYield * Find.Storyteller.difficulty.mineYieldFactor * 0.2f));
                Thing thing = ThingMaker.MakeThing(thingDef.building.mineableThing, null);
                thing.stackCount = num;
                GenSpawn.Spawn(thing, pawn.Position, map, WipeMode.Vanish);
                if ((pawn == null || !pawn.IsColonist) && thing.def.EverHaulable && !thing.def.designateHaulable)
                {
                    thing.SetForbidden(true, true);
                }
            }
        }
        public static float FactorFromHilliness(Hilliness hilliness)
        {
            if (hilliness == Hilliness.Flat)
            {
                return 0.2f;
            }
            else if (hilliness == Hilliness.SmallHills)
            {
                return 0.12f;
            }
            else if (hilliness == Hilliness.LargeHills)
            {
                return 0.1f;
            }
            else if (hilliness == Hilliness.Mountainous)
            {
                return 0.05f;
            }
            else
            {
                return 0.05f;
            }
        }
    }

    //Da Vinci Gene
    [HarmonyPatch(typeof(StatWorker), nameof(StatWorker.GetValueUnfinalized))]
    public static class GetValueUnfinalizedPatch
    {
        [HarmonyPostfix]
        public static void GetValueUnfinalized_Postfix(ref float __result, StatWorker __instance, StatRequest req, StatDef ___stat)
        {
            if (req.Thing != null)
            {
                Pawn pawn = req.Thing as Pawn;
                if (pawn?.story?.traits != null && ___stat != null && pawn.story.traits.HasTrait(SyrTraitDefOf.SYR_CreativeThinker) && ___stat == StatDefOf.ResearchSpeed)
                {
                    if (pawn.skills != null && pawn.def.statBases != null)
                    {
                        float statBase = 1f;
                        if (pawn.def.statBases.Find((StatModifier x) => x?.stat != null && x.stat == ___stat) != null)
                        {
                            statBase = pawn.def.statBases.Find((StatModifier x) => x?.stat != null && x.stat == ___stat).value;
                        }
                        __result += 0.115f * pawn.skills.GetSkill(SkillDefOf.Artistic).Level * statBase;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(StatWorker), nameof(StatWorker.GetExplanationUnfinalized))]
    public static class GetExplanationUnfinalizedPatch
    {
        [HarmonyPostfix]
        public static void GetExplanationUnfinalized_Postfix(ref string __result, StatWorker __instance, StatRequest req, ToStringNumberSense numberSense, StatDef ___stat)
        {
            if (req.Thing != null)
            {
                Pawn pawn = req.Thing as Pawn;
                if (pawn?.story?.traits != null && ___stat != null && pawn.story.traits.HasTrait(SyrTraitDefOf.SYR_CreativeThinker) && ___stat == StatDefOf.ResearchSpeed)
                {
                    if (pawn.skills != null && pawn.def.statBases != null)
                    {
                        float statBase = 1f;
                        if (pawn.def.statBases.Find((StatModifier x) => x?.stat != null && x.stat == ___stat) != null)
                        {
                            statBase = pawn.def.statBases.Find((StatModifier x) => x?.stat != null && x.stat == ___stat).value;
                        }
                        float val = 0.115f * pawn.skills.GetSkill(SkillDefOf.Artistic).Level * statBase;
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.AppendLine("SyrTraitsDaVinciGene".Translate());
                        stringBuilder.AppendLine(string.Concat(new object[]
                                {
                                "    " + SkillDefOf.Artistic.LabelCap + " (",
                                pawn.skills.GetSkill(SkillDefOf.Artistic).Level,
                                "): ",
                                val.ToStringSign(),
                                __instance.ValueToString(val, false, ToStringNumberSense.Absolute)
                                }));
                        __result += stringBuilder.ToString();
                    }
                }
            }
        }
    }

    //Uninspired
    [HarmonyPatch(typeof(StatWorker), nameof(StatWorker.GetValueUnfinalized))]
    public static class GetValueUnfinalizedPatch2
    {
        [HarmonyPostfix]
        public static void GetValueUnfinalized_Postfix2(ref float __result, StatWorker __instance, StatRequest req, StatDef ___stat)
        {
            if (req.Thing != null)
            {
                Pawn pawn = req.Thing as Pawn;
                if (pawn?.story?.traits != null && ___stat != null && pawn.story.traits.HasTrait(SyrTraitDefOf.SlowLearner) && ___stat == StatDefOf.GlobalLearningFactor)
                {
                    if (pawn.skills?.skills != null && pawn.def.statBases != null)
                    {
                        float num = 0f;
                        foreach (SkillRecord skillRecord in pawn.skills.skills)
                        {
                            num += skillRecord.levelInt;
                        }
                        __result += 0.02f * Mathf.Clamp(num, 40, 140);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(StatWorker), nameof(StatWorker.GetExplanationUnfinalized))]
    public static class GetExplanationUnfinalizedPatch2
    {
        [HarmonyPostfix]
        public static void GetExplanationUnfinalized_Postfix2(ref string __result, StatWorker __instance, StatRequest req, ToStringNumberSense numberSense, StatDef ___stat)
        {
            if (req.Thing != null)
            {
                Pawn pawn = req.Thing as Pawn;
                if (pawn?.story?.traits != null && ___stat != null && pawn.story.traits.HasTrait(SyrTraitDefOf.SlowLearner) && ___stat == StatDefOf.GlobalLearningFactor)
                {
                    if (pawn.skills != null && pawn.def.statBases != null)
                    {
                        float num = 0f;
                        foreach (SkillRecord skillRecord in pawn.skills.skills)
                        {
                            num += skillRecord.levelInt;
                        }
                        float val = 0.02f * Mathf.Clamp(num, 40, 140);
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.AppendLine("SyrTraitsSlowLearnerLabel".Translate());
                        stringBuilder.AppendLine(string.Concat(new object[]
                                {
                                "    " + "SyrTraitsSlowLearner".Translate() + " (",
                                num,
                                "): ",
                                val.ToStringSign(),
                                __instance.ValueToString(val, false, ToStringNumberSense.Absolute)
                                }));
                        __result += stringBuilder.ToString();
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Trait), nameof(Trait.TipString))]
    public static class TraitTipStringPatch
    {
        [HarmonyPrefix]
        public static bool TraitTipString_Prefix(ref string __result, Trait __instance, Pawn pawn)
        {
            if (__instance?.def != null && __instance.def == SyrTraitDefOf.SlowLearner)
            {
                StringBuilder stringBuilder = new StringBuilder();
                TraitDegreeData currentData = __instance.CurrentData;
                stringBuilder.Append(currentData.description.Formatted(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN", true).Resolve());
                if (pawn.skills != null && pawn.def.statBases != null)
                {
                    float num = 0f;
                    foreach (SkillRecord skillRecord in pawn.skills.skills)
                    {
                        num += skillRecord.levelInt;
                    }
                    float val = (SyrTraitDefOf.SlowLearner.degreeDatas.First().statOffsets.Find((StatModifier x) => x?.stat != null && x.stat == StatDefOf.GlobalLearningFactor).value + 0.02f * Mathf.Clamp(num, 40, 140)) * 100;
                    string value = "    " + StatDefOf.GlobalLearningFactor.LabelCap + " " + val.ToStringWithSign() + "%";
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(value);
                }
                __result = stringBuilder.ToString();
                return false;
            }
            return true;
        }
    }

    //Mechanoid Expert
    [HarmonyPatch(typeof(DamageWorker_AddInjury), "FinalizeAndAddInjury", new Type[]{ typeof(Pawn), typeof(float), typeof(DamageInfo), typeof(DamageWorker.DamageResult) })]
    public static class FinalizeAndAddInjuryPatch
    {
        [HarmonyPrefix]
        public static void FinalizeAndAddInjury_Prefix(Pawn pawn, ref float totalDamage, DamageInfo dinfo, DamageWorker.DamageResult result)
        {
            if (dinfo.Instigator != null)
            {
                Pawn dealer = dinfo.Instigator as Pawn;
                if (dealer?.story?.traits != null && dealer.story.traits.HasTrait(SyrTraitDefOf.SYR_MechanoidExpert) && pawn?.def?.race?.FleshType != null && pawn.def.race.IsMechanoid)
                {
                    totalDamage *= 1.5f;
                }
            }
        }
    }

    //Perfectionist
    [HarmonyPatch(typeof(QualityUtility), nameof(QualityUtility.GenerateQualityCreatedByPawn), new Type[] { typeof(Pawn), typeof(SkillDef) })]
    public class GenerateQualityCreatedByPawnPatch
    {
        [HarmonyPostfix]
        [HarmonyPriority(Priority.Last)]
        public static void GenerateQualityCreatedByPawn_Postfix(ref QualityCategory __result, Pawn pawn, SkillDef relevantSkill)
        {
            if (pawn?.story?.traits!= null && pawn.story.traits.HasTrait(SyrTraitDefOf.SYR_Perfectionist))
            {
                __result += 1;
                if (__result > QualityCategory.Legendary)
                {
                    __result = QualityCategory.Legendary;
                }
            }
        }
    }

    //AnimalTrait
    [HarmonyPatch(typeof(InteractionWorker_RecruitAttempt), nameof(InteractionWorker_RecruitAttempt.Interacted))]
    public static class Recruit_InteractedPatch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Recruit_Interacted_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            FieldInfo wildness = AccessTools.Field(typeof(RaceProperties), "wildness");
            MethodInfo modifyWildness = AccessTools.Method(typeof(Recruit_InteractedPatch), nameof(Recruit_InteractedPatch.ModifyWildness));
            foreach (CodeInstruction i in instructions)
            {
                if (i.opcode == OpCodes.Ldfld && (FieldInfo)i.operand == wildness)
                {
                    yield return i;
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Call, modifyWildness);
                }
                else
                {
                    yield return i;
                }
            }
        }
        public static float ModifyWildness(float wildness, Pawn pawn)
        {
            if (pawn?.story?.traits != null && pawn.story.traits.HasTrait(SyrTraitDefOf.SYR_AnimalAffinity) && wildness < 1f)
            {
                return Mathf.Clamp(wildness * 0.9f, 0, 2);
            }
            return wildness;
        }
    }

    [HarmonyPatch(typeof(Pawn_MindState), "CheckStartMentalStateBecauseRecruitAttempted")]
    public static class MentalStateBecauseRecruitAttemptedPatch
    {
        [HarmonyPrefix]
        public static bool MentalStateBecauseRecruitAttempted_Prefix(ref bool __result, Pawn_MindState __instance, Pawn tamer)
        {
            if (tamer?.story?.traits != null && tamer.story.traits.HasTrait(SyrTraitDefOf.SYR_AnimalAffinity))
            {
                __result = false;
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    //SteadyHands
    [HarmonyPatch(typeof(TendUtility), nameof(TendUtility.CalculateBaseTendQuality), new Type[] { typeof(Pawn), typeof(Pawn), typeof(float), typeof(float) })]
    public static class CalculateBaseTendQualityPatch
    {
        [HarmonyPrefix]
        public static bool CalculateBaseTendQuality_Prefix(ref float __result, Pawn doctor, Pawn patient, float medicinePotency, float medicineQualityMax)
        {
            if (doctor?.story?.traits != null && doctor.story.traits.HasTrait(SyrTraitDefOf.SYR_SteadyHands))
            {
                float num;
                if (doctor != null)
                {
                    num = doctor.GetStatValue(StatDefOf.MedicalTendQuality, true);
                }
                else
                {
                    num = 0.75f;
                }
                num *= medicinePotency;
                Building_Bed building_Bed = (patient != null) ? patient.CurrentBed() : null;
                if (building_Bed != null)
                {
                    num += building_Bed.GetStatValue(StatDefOf.MedicalTendQualityOffset, true);
                }
                __result = Mathf.Clamp(num, 0f, medicineQualityMax);
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    //Architect
    public static class JobDriver_ConstructFinishFramePatch
    {
        public static IEnumerable<CodeInstruction> JobDriver_ConstructFinishFrame_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo GetStatValueAbstract = AccessTools.Method(typeof(StatExtension), nameof(StatExtension.GetStatValueAbstract), new Type[] { typeof(BuildableDef), typeof(StatDef), typeof(ThingDef) });
            MethodInfo modifyConstructionSpeed = AccessTools.Method(typeof(JobDriver_ConstructFinishFramePatch), nameof(JobDriver_ConstructFinishFramePatch.ModifyConstructionSpeed));
            MethodInfo getStuff = AccessTools.Property(typeof(Thing), nameof(Thing.Stuff)).GetGetMethod();
            bool found = false;
            foreach (CodeInstruction i in instructions)
            {
                yield return i;
                if (i.opcode == OpCodes.Call && (MethodInfo)i.operand == GetStatValueAbstract)
                {
                    found = true;
                }
                if (found && i.opcode == OpCodes.Mul)
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_0); //pawn
                    yield return new CodeInstruction(OpCodes.Ldloc_1); //frame
                    yield return new CodeInstruction(OpCodes.Call, modifyConstructionSpeed); //call my method
                    yield return new CodeInstruction(OpCodes.Mul); //multiply previous value on stack with float from my method
                    found = false;
                }
            }
        }
        public static float ModifyConstructionSpeed (Pawn pawn, Frame frame)
        {
            if (pawn?.story?.traits != null && frame?.Stuff?.stuffProps?.categories != null && pawn.story.traits.HasTrait(SyrTraitDefOf.SYR_Architect) && frame.Stuff.stuffProps.categories.Contains(StuffCategoryDefOf.Stony))
            {
                return 3f;
            }
            return 1f;
        }
    }
}
