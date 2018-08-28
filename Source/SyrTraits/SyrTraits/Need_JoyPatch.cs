using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using RimWorld;
using Verse;
using System.Reflection.Emit;

namespace SyrTraits
{
    [HarmonyPatch(typeof(Need_Joy), "NeedInterval")]
    public static class Need_JoyPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo get_FallPerInterval = AccessTools.Method(typeof(Need_Joy), "get_FallPerInterval");
            MethodInfo GetJoyNeedRateMultiplier = AccessTools.Method(typeof(Need_JoyPatch), nameof(GetJoyNeedRateMultiplier));
            FieldInfo pawn = AccessTools.Field(typeof(Need), "pawn");
            foreach (CodeInstruction i in instructions)
            {
                if (i.opcode == OpCodes.Call && i.operand == get_FallPerInterval)
                {
                    yield return i;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);//get object
                    yield return new CodeInstruction(OpCodes.Ldfld, pawn);//get pawn
                    yield return new CodeInstruction(OpCodes.Call, GetJoyNeedRateMultiplier);//get the stat of the pawn as float
                    yield return new CodeInstruction(OpCodes.Mul);//multiply with FallPerInterval
                }
                else
                {
                    yield return i;
                }
            }
        }

        public static float GetJoyNeedRateMultiplier(Pawn pawn)
        {
            return pawn.GetStatValue(SyrTraitDefOf.JoyNeedRateMultiplier);
        }
    }
}
