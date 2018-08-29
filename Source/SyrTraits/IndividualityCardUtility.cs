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
                Text.Font = GameFont.Medium;
                Rect rect1 = new Rect(20f, 10f, rect.width - 20f, rect.height - 30f);
                Widgets.Label(rect1, "IndividualityWindow".Translate());
                Text.Font = GameFont.Small;
                float num = rect1.y + 30f;
                Rect rect2 = new Rect(20f, num, rect.width - 20f, 24f);
                Widgets.Label(rect2, "SexualityPawn".Translate() + ": " + pawn.TryGetComp<CompIndividuality>().sexuality);
                TipSignal SexualityPawnTooltip = "SexualityPawnTooltip".Translate();
                TooltipHandler.TipRegion(rect2, SexualityPawnTooltip);
                num += rect2.height + 2f;
                //if (Prefs.DevMode)
                //{
                    Rect rect3 = new Rect(20f, num, rect.width - 20f, 24f);
                    Widgets.Label(rect3, "RomanceFactor".Translate() + ": " + (pawn.TryGetComp<CompIndividuality>().RomanceFactor * 10));
                    TipSignal RomanceFactorTooltip = "RomanceFactorTooltip".Translate();
                    TooltipHandler.TipRegion(rect3, RomanceFactorTooltip);
                    num += rect3.height + 2f;
                //}
                Rect rect4 = new Rect(20f, num, rect.width - 20f, 24f);
                Widgets.Label(rect4, "BodyWeight".Translate() + ": " + (pawn.TryGetComp<CompIndividuality>().BodyWeight + 70) + " kg (" + pawn.story.bodyType + ")");
                TipSignal BodyWeightTooltip = "BodyWeightTooltip".Translate();
                TooltipHandler.TipRegion(rect4, BodyWeightTooltip);
                num += rect4.height + 2f;
                Rect rect5 = new Rect(20f, num, rect.width - 20f, 24f);
                Widgets.Label(rect5, "PsychicFactor".Translate() + ": " + pawn.TryGetComp<CompIndividuality>().PsychicFactor.ToStringByStyle(ToStringStyle.PercentZero, ToStringNumberSense.Offset));
                TipSignal PsychicFactorTooltip = "PsychicFactorTooltip".Translate();
                TooltipHandler.TipRegion(rect5, PsychicFactorTooltip);
                //GUI.EndGroup();
            }
        }
    }
}
