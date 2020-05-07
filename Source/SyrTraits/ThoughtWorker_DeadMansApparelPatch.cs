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
    [HarmonyPatch(typeof(ThoughtWorker_DeadMansApparel), "CurrentStateInternal")]
    public static class ThoughtWorker_DeadMansApparelPatch
    {
        [HarmonyPostfix]
        public static void ThoughtWorker_DeadMansApparel_Postfix(ref ThoughtState __result, Pawn p)
        {
            if (p.story.traits.HasTrait(SyrTraitDefOf.Jealous))
            {
                __result = ThoughtState.Inactive;
            }
        }
    }
}
