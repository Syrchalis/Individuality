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
    [HarmonyPatch(typeof(TraitSet), "GainTrait")]
    public static class TraitSet_GainTraitPatch
    {
        [HarmonyPostfix]
        public static void TraitSet_GainTrait_Postfix(ref Trait trait, TraitSet __instance)
        {
            if (trait.def == TraitDefOf.Gay || trait.def == TraitDefOf.PsychicSensitivity)
            {
                __instance.allTraits.Remove(trait);
            }
        }
    }
}
