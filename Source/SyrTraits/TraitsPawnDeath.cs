using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;

namespace SyrTraits
{
    [HarmonyPatch(typeof(PawnDiedOrDownedThoughtsUtility), "AppendThoughts_ForHumanlike")]
    public class TraitsPawnDeath
    {
        [HarmonyPostfix]
        public static void AppendThoughts_ForHumanlike_Postfix(Pawn victim, DamageInfo? dinfo, PawnDiedOrDownedThoughtsKind thoughtsKind, List<IndividualThoughtToAdd> outIndividualThoughts, List<ThoughtDef> outAllColonistsThoughts)
        {
            bool flag = dinfo != null && dinfo.Value.Def.execution;
            if (thoughtsKind == PawnDiedOrDownedThoughtsKind.Died && !flag)
            {
                foreach (Pawn pawn2 in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive)
                {
                    if (pawn2 != victim && pawn2.needs.mood != null)
                    {
                        if (pawn2.MentalStateDef != MentalStateDefOf.SocialFighting || ((MentalState_SocialFighting)pawn2.MentalState).otherPawn != victim)
                        {
                            if (Witnessed(pawn2, victim))
                            {
                                outIndividualThoughts.Add(new IndividualThoughtToAdd(SyrTraitDefOf.WitnessedDeathMedic, pawn2, null, 1f, 1f));
                            }
                        }
                    }
                }
            }
        }
        private static bool Witnessed(Pawn p, Pawn victim)
        {
            bool result;
            if (!p.Awake() || !p.health.capacities.CapableOf(PawnCapacityDefOf.Sight))
            {
                result = false;
            }
            else if (victim.IsCaravanMember())
            {
                result = (victim.GetCaravan() == p.GetCaravan());
            }
            else
            {
                result = (victim.Spawned && p.Spawned && p.Position.InHorDistOf(victim.Position, 12f) && GenSight.LineOfSight(victim.Position, p.Position, victim.Map, false, null, 0, 0));
            }
            return result;
        }
    }
}
