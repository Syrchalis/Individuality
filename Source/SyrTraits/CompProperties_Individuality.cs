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
    public class CompProperties_Individuality : CompProperties
    {
        public CompProperties_Individuality()
        {
            compClass = typeof(CompIndividuality);
        }
    }
}
