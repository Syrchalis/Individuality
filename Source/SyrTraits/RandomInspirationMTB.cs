using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using RimWorld;
using Verse;

namespace SyrTraits
{
    public class RandomInspirationMtbDays : TraitDegreeData
    {
        public float randomInspirationMtbDays = 0f;
    }
    [HarmonyPatch(typeof(InspirationHandler), "InspirationHandlerTick")]
    public class RandomInspirationMtb
    {
        [HarmonyPostfix]
        public static void RandomInspirationMtb_Postfix(InspirationHandler __instance)
        {
            if (__instance.pawn.IsHashIntervalTick(100))
            {
                CheckStartTrait_RandomInspiration(__instance);
            }
        }
        public static void CheckStartTrait_RandomInspiration(InspirationHandler __instance)
        {
            if (!__instance.Inspired && __instance?.pawn?.story != null)
            {
                List<Trait> allTraits = __instance.pawn.story.traits.allTraits;
                for (int m = 0; m < allTraits.Count; m++)
                {
                    RandomInspirationMtbDays currentData = allTraits[m].CurrentData as RandomInspirationMtbDays;
                    if (currentData != null && currentData.randomInspirationMtbDays > 0f && Rand.MTBEventOccurs(currentData.randomInspirationMtbDays, 60000f, 100f))
                    {
                        InspirationDef randomAvailableInspirationDef = GetRandomAvailableInspirationDef(__instance);
                        if (randomAvailableInspirationDef != null)
                        {
                            __instance.TryStartInspiration(randomAvailableInspirationDef);
                        }
                    }
                }
            }
        }
        private static InspirationDef GetRandomAvailableInspirationDef(InspirationHandler __instance)
        {
            return (from x in DefDatabase<InspirationDef>.AllDefsListForReading
                    where x.Worker.InspirationCanOccur(__instance.pawn) && (x != InspirationDefOf.Inspired_Surgery) && (x != SyrTraitDefOf.Frenzy_Shoot)
                    select x).RandomElementByWeightWithFallback((InspirationDef x) => x.Worker.CommonalityFor(__instance.pawn), null);
        }
    }
}
