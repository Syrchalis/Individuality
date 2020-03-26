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
    [DefOf]
    public static class SyrTraitDefOf
    {
        static SyrTraitDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(SyrTraitDefOf));
        }
        public static InspirationDef Frenzy_Shoot;
        public static StatDef JoyNeedRateMultiplier;
        public static TraitDef Jealous;
        public static TraitDef Neurotic;
        public static TraitDef SlowLearner;

        public static TraitDef SYR_GreenThumb;

        public static TraitDef SYR_Blacksmith;
        public static TraitDef SYR_KeenEye;

        public static TraitDef SYR_Scientist;
        public static TraitDef SYR_CreativeThinker;

        public static TraitDef SYR_Tinkerer;
        public static TraitDef SYR_MechanoidExpert;

        public static TraitDef SYR_Architect;
        public static TraitDef SYR_Perfectionist;

        public static TraitDef SYR_Chef;
        public static TraitDef Gourmand;

        public static TraitDef SYR_MedicalTraining;
        public static TraitDef SYR_SteadyHands;

        public static TraitDef SYR_Hotblooded;
        public static TraitDef SYR_HandEyeCoordination;

        public static TraitDef SYR_Haggler;
        public static TraitDef SYR_AnimalAffinity;
    }
}
