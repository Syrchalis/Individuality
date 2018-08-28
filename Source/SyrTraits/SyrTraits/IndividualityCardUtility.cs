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
    public static class IndividualityCardUtility
    {
        public static void DrawIndividualityCard(Rect rect, Pawn pawn)
        {
            if (pawn != null)
            {
                //GUI.BeginGroup(rect);
                Text.Font = GameFont.Small;
                float num = 20f;
                Rect rect2 = new Rect(20f, num, rect.width - 20f, rect.height - 20f);
                //GUI.color = new Color(1f, 1f, 1f, 0.5f);
                Widgets.Label(rect2, "SexualityPawn".Translate() + ": " + pawn.TryGetComp<CompIndividuality>().sexuality);
                num += 20f;
                Rect rect3 = new Rect(20f, num, rect.width - 20f, rect.height - 20f);
                Widgets.Label(rect3, "BodyWeight".Translate() + ": " + pawn.TryGetComp<CompIndividuality>().BodyWeight + " (" + pawn.story.bodyType + ")");
                num += 20f;

                if (Prefs.DevMode)
                {
                    Rect rect4 = new Rect(20f, num, rect.width - 20f, rect.height - 20f);
                    Widgets.Label(rect4, "RomanceFactor".Translate() + ": " + pawn.TryGetComp<CompIndividuality>().RomanceFactor);
                    num += 20f;
                }
                Rect rect5 = new Rect(20f, num, rect.width - 20f, rect.height - 20f);
                Widgets.Label(rect5, "PsychicFactor".Translate() + ": " + pawn.TryGetComp<CompIndividuality>().PsychicFactor);
                //GUI.EndGroup();
            }
        }
    }
}
