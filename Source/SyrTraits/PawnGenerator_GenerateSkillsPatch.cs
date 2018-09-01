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
    [HarmonyPatch(typeof(PawnGenerator), "GenerateSkills")]
    public static class PawnGenerator_GenerateSkillsPatch
    {
        [HarmonyPostfix]
        public static void PawnGenerator_GenerateSkills_Postfix(Pawn pawn)
        {
            if (SyrIndividualitySettings.traitsForcePassion)
            {
                ForcePassion(pawn, SyrTraitDefOf.SYR_AnimalAffinity, SkillDefOf.Animals, SkillDefOf.Social);
                ForcePassion(pawn, SyrTraitDefOf.SYR_Architect, SkillDefOf.Construction, SkillDefOf.Intellectual);
                ForcePassion(pawn, SyrTraitDefOf.SYR_Chef, SkillDefOf.Cooking, SkillDefOf.Crafting);
                ForcePassion(pawn, SyrTraitDefOf.SYR_Digger, SkillDefOf.Mining, SkillDefOf.Plants);
                ForcePassion(pawn, SyrTraitDefOf.SYR_GreenThumb, SkillDefOf.Plants, SkillDefOf.Mining);
                ForcePassion(pawn, SyrTraitDefOf.SYR_Haggler, SkillDefOf.Social, SkillDefOf.Animals);
                ForcePassion(pawn, SyrTraitDefOf.SYR_MedicalTraining, SkillDefOf.Medicine);
                ForcePassion(pawn, SyrTraitDefOf.SYR_Peculiar, SkillDefOf.Artistic, SkillDefOf.Intellectual);
                ForcePassion(pawn, SyrTraitDefOf.SYR_Scientist, SkillDefOf.Intellectual, SkillDefOf.Construction);
                ForcePassion(pawn, SyrTraitDefOf.SYR_Tinkerer, SkillDefOf.Crafting, SkillDefOf.Cooking);
                if (pawn.story.traits.HasTrait(TraitDefOf.ShootingAccuracy) && pawn.story.traits.DegreeOfTrait(TraitDefOf.ShootingAccuracy) == 2)
                {
                    if (Rand.Value > 0.66f)
                        pawn.skills.GetSkill(SkillDefOf.Shooting).passion = Passion.Major;
                    else
                        pawn.skills.GetSkill(SkillDefOf.Shooting).passion = Passion.Minor;
                }
            }
        }

        private static void ForcePassion(Pawn pawn, TraitDef thisTrait, SkillDef thisSkill, SkillDef badSkill = null)
        {
            if (pawn.story.traits.HasTrait(thisTrait))
            {
                if (Rand.Value > 0.66f)
                {
                    pawn.skills.GetSkill(thisSkill).passion = Passion.Major;
                } 
                else
                {
                    pawn.skills.GetSkill(thisSkill).passion = Passion.Minor;
                }
                Log.Message("Changed passion of " + thisSkill + " on " + pawn);
                if (badSkill != null)
                {
                    pawn.skills.GetSkill(badSkill).passion = Passion.None;
                }  
            }
        }
    }
}
