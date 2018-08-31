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
    [StaticConstructorOnStartup]
    public static class CompToHumanlikes
    {
        static CompToHumanlikes()
        {
            AddCompToHumanlikes();
        }

        public static void AddCompToHumanlikes()
        {
            foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs)
            {
                if (thingDef.race != null && thingDef.race.intelligence == Intelligence.Humanlike)
                {
                    thingDef.comps.Add(new CompProperties_Individuality());
                }
            }
        }
    }
}
