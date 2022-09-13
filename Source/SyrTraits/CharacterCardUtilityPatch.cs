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

            MethodInfo IndividualityCardButton = AccessTools.Method(typeof(CharacterCardUtilityPatch), nameof(IndividualityCardButton));
            var codes = instructions.ToList();
            var idx1 = codes.FindIndex(ins => ins.IsStloc() && ins.operand is LocalBuilder { LocalIndex: 15 });
            var list = codes.GetRange(idx1 + 1, 2).Select(ins => ins.Clone()).ToList();
            codes.InsertRange(idx1 + 1, list.Concat(new List<CodeInstruction>
            {
                new CodeInstruction(OpCodes.Ldloca, 15),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldarg_3),                
                new CodeInstruction(OpCodes.Call,IndividualityCardButton)
            }));
            return codes;
        }


        public static void IndividualityCardButton(ref float x, Rect rect, Pawn pawn, Rect creationRect)
        {
            if (pawn != null)
            {
                TipSignal tooltip = "IndividualityTooltip".Translate();

                Rect rectNew = new Rect(x, 1f, 24f, 24f);
                x -= 40f;
                if (Current.ProgramState != ProgramState.Playing)
                {
                    rectNew = new Rect(creationRect.width - 24f, 80f, 24f, 24f);
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
