using HarmonyLib;
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
    public class ThoughtWorker_GreenThumb : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (!p.IsColonist)
            {
                return ThoughtState.Inactive;
            }
            Room bedroom = p.ownership.OwnedRoom;
            if (bedroom == null)
            {
                return ThoughtState.ActiveAtStage(0);
            }
            int num = 0;
            foreach (ThingDef thingDef in HarmonyPatches.beautyPlants)
            {
                num += bedroom.ThingCount(thingDef);
            }
            return ThoughtState.ActiveAtStage(Mathf.RoundToInt(Mathf.Clamp((num + 1) / 2, 0f, 5f)));
        }
    }
}