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
    public class SyrIndividualitySettings : ModSettings
    {
        public static float commonalityStraight = 0.8f;
        public static float commonalityBi = 0.1f;
        public static float commonalityGay = 0.1f;
        public static IntRange traitCount = new IntRange(2, 3);
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref commonalityStraight, "commonalityStraight", 0.8f, true);
            Scribe_Values.Look<float>(ref commonalityBi, "commonalityBi", 0.1f, true);
            Scribe_Values.Look<float>(ref commonalityGay, "commonalityGay", 0.1f, true);
            Scribe_Values.Look<IntRange>(ref traitCount, "traitCount", new IntRange(2, 3), true);
        }
    }
}