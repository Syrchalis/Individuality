using Harmony;
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
                }
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
                float num = CharacterCardUtility.PawnCardSize.x - 163f;
                Rect rectNew = new Rect(num, 2f, 24f, 24f);
                if (Current.ProgramState != ProgramState.Playing)
                {
                    rectNew = new Rect(creationRect.width - 54f - 100f, 0f, 24f, 24f);
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
                if (Widgets.ButtonInvisible(rectNew, false))
                {
                    SoundDefOf.InfoCard_Open.PlayOneShotOnCamera(null);
                    Find.WindowStack.Add(new Dialog_ViewIndividuality(pawn));
                }
                GUI.color = old;
            }
        }
    }
}
