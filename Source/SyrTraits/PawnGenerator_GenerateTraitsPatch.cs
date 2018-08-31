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
    [HarmonyPatch(typeof(PawnGenerator), "GenerateTraits", null)]
    public static class PawnGenerator_GenerateTraitsPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo RangeInclusive = AccessTools.Method(typeof(Rand), "RangeInclusive");
            FieldInfo IntRange = AccessTools.Field(typeof(SyrIndividualitySettings), "traitCount");
            FieldInfo min = AccessTools.Field(typeof(IntRange), "min");
            FieldInfo max = AccessTools.Field(typeof(IntRange), "max");
            foreach (CodeInstruction i in instructions)
            {
                if (i.opcode == OpCodes.Call && i.operand == RangeInclusive)
                {
                    yield return new CodeInstruction(OpCodes.Pop);//pop first int value we don't want
                    yield return new CodeInstruction(OpCodes.Pop);//pop second int value we don't want
                    yield return new CodeInstruction(OpCodes.Ldsfld, IntRange);//loads the static int range into the stack
                    yield return new CodeInstruction(OpCodes.Ldfld, min);//gets the min field out of the int range which is on the stack
                    yield return new CodeInstruction(OpCodes.Ldsfld, IntRange);//loads the static int range into the stack
                    yield return new CodeInstruction(OpCodes.Ldfld, max);//gets the max field out of the int range which is on the stack
                    yield return i;
                }
                else
                {
                    yield return i;
                }
            }
        }
    }
}

