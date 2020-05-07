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
    public class ThoughtWorker_IsCarryingMeleeWeapon : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            return p.equipment.Primary != null && p.equipment.Primary.def.IsMeleeWeapon;
        }
    }
}
