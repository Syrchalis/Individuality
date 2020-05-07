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
    public class SyrIndividualitySettings : ModSettings
    {
        public static float commonalityStraight = 0.8f;
        public static float commonalityBi = 0.1f;
        public static float commonalityGay = 0.1f;
        public static float commonalityAsexual = 0f;
        public static IntRange traitCount = new IntRange(2, 3);
        public static bool disableRomance = false;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref commonalityStraight, "Individuality_CommonalityStraight", 0.8f, true);
            Scribe_Values.Look<float>(ref commonalityBi, "Individuality_CommonalityBi", 0.1f, true);
            Scribe_Values.Look<float>(ref commonalityGay, "Individuality_CommonalityGay", 0.1f, true);
            Scribe_Values.Look<float>(ref commonalityAsexual, "Individuality_CommonalityAsexual", 0.1f, true);
            Scribe_Values.Look<IntRange>(ref traitCount, "Individuality_TraitCount", new IntRange(2, 3), true);
            Scribe_Values.Look<bool>(ref disableRomance, "Individuality_DisableRomance", false, true);
        }
    }
}