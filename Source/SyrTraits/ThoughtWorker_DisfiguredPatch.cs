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
    [HarmonyPatch(typeof(ThoughtWorker_Disfigured), "CurrentSocialStateInternal")]
    public static class ThoughtWorker_DisfiguredPatch
    {
        [HarmonyPostfix]
        public static void ThoughtWorker_Disfigured_Postfix(ref ThoughtState __result, Pawn pawn, Pawn other)
        {
            if (other?.story?.traits != null && other.story.traits.HasTrait(TraitDefOf.Beauty))
            {
                int num = other.story.traits.DegreeOfTrait(TraitDefOf.Beauty);
                if (num == -1 || num == -2)
                {
                    __result = false;
                }
            }
        }
    }
}