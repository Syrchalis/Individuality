using Harmony;
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
        public static ThoughtDef WitnessedDeathMedic;
        public static InspirationDef Frenzy_Shoot;
        public static StatDef JoyNeedRateMultiplier;
        public static TraitDef Jealous;
        public static TraitDef Neurotic;
    }
}
