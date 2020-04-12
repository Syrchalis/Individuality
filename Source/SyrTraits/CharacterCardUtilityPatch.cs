using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using RimWorld;
using Verse;
using UnityEngine;
using Verse.Sound;
using System.Reflection.Emit;

namespace SyrTraits
{
    [HarmonyPatch(typeof(CharacterCardUtility), "DrawCharacterCard")]
    public static class CharacterCardUtilityPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo HighlightOpportunity = AccessTools.Method(typeof(UIHighlighter), "HighlightOpportunity");
            MethodInfo IndividualityCardButton = AccessTools.Method(typeof(CharacterCardUtilityPatch), nameof(IndividualityCardButton));
            MethodInfo SetTextSize = AccessTools.Method(typeof(CharacterCardUtilityPatch), nameof(SetTextSize));
            MethodInfo SetRectSize = AccessTools.Method(typeof(CharacterCardUtilityPatch), nameof(SetRectSize));
            //bool traits = false;
            bool found = false;
            foreach (CodeInstruction i in instructions)
            {
                if (found)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0) { labels = i.labels.ListFullCopy() };//rect
                    yield return new CodeInstruction(OpCodes.Ldarg_1);//pawn
                    yield return new CodeInstruction(OpCodes.Ldarg_3);//creationRect
                    yield return new CodeInstruction(OpCodes.Call, IndividualityCardButton);
                    found = false;
                    i.labels.Clear();
                }
                if (i.opcode == OpCodes.Call && i.operand == HighlightOpportunity)
                {
                    found = true;
                }/*
                if (i.opcode == OpCodes.Ldstr && i.operand.Equals("Traits"))
                {
                    traits = true;
                }
                if (traits && i.opcode == OpCodes.Ldc_I4_1)
                {
                    yield return new CodeInstruction(OpCodes.Call, SetTextSize);//replaces instruction, gets 0 or 1 returned from the method, depending on setting
                    continue;
                }
                if (traits && i.opcode == OpCodes.Ldc_R4 && i.operand.Equals(24f))//replaces rect height
                {
                    yield return new CodeInstruction(OpCodes.Call, SetRectSize);
                    continue;
                }
                if (traits && i.opcode == OpCodes.Ldc_R4 && i.operand.Equals(2f))//replaces rect y calculation
                {
                    yield return new CodeInstruction(OpCodes.Ldc_R4, 0f);
                    traits = false;
                    continue;
                }*/
                yield return i;
            }
        }

        /*[HarmonyPostfix]
        public static void DrawCharacterCard_Postfix(Rect rect, Pawn pawn, Action randomizeCallback = null, Rect creationRect = default(Rect))
        {
            float num = CharacterCardUtility.PawnCardSize.x - 182f;
            Rect rect0 = new Rect(num, 0f, 30f, 60f);
            IndividualityCardButton(rect0, pawn);
        }*/

        public static void IndividualityCardButton(Rect rect, Pawn pawn, Rect creationRect)
        {
            if (pawn != null)
            {
                TipSignal tooltip = "IndividualityTooltip".Translate();
                float num = CharacterCardUtility.BasePawnCardSize.x - 50f;
                Rect rectNew = new Rect(num, 34f, 24f, 24f);
                if (Current.ProgramState != ProgramState.Playing)
                {
                    rectNew = new Rect(creationRect.width - 24f, 30f, 24f, 24f);
                }
                Color old = GUI.color;
                if (rectNew.Contains(Event.current.mousePosition))
                {
                    GUI.color = new Color(0.25f, 0.59f, 0.75f);
                }
                else
                {
                    GUI.color = new Color(1f, 1f, 1f);
                }
                GUI.DrawTexture(rectNew, ContentFinder<Texture2D>.Get("Buttons/Individuality", true));
                TooltipHandler.TipRegion(rectNew, tooltip);
                if (Widgets.ButtonInvisible(rectNew))
                {
                    SoundDefOf.InfoCard_Open.PlayOneShotOnCamera(null);
                    Find.WindowStack.Add(new Dialog_ViewIndividuality(pawn));
                }
                GUI.color = old;
            }
        }
    }
}
