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
            CompIndividuality comp = pawn.TryGetComp<CompIndividuality>();
            if (pawn != null && comp != null)
            {
                //GUI.BeginGroup(rect);
                
                Text.Font = GameFont.Medium;
                Rect rect1 = new Rect(20f, 10f, rect.width - 20f, rect.height - 30f);
                Widgets.Label(rect1, "IndividualityWindow".Translate() + " - " + pawn.Name.ToStringShort);
                Text.Font = GameFont.Small;
                float num = rect1.y + 30f;
                if (!SyrIndividuality.PsychologyIsActive)
                {
                    Rect rect2 = new Rect(20f, num, rect.width - 20f, 24f);
                    Widgets.Label(rect2, "SexualityPawn".Translate() + ": " + comp.sexuality);
                    TipSignal SexualityPawnTooltip = "SexualityPawnTooltip".Translate();
                    TooltipHandler.TipRegion(rect2, SexualityPawnTooltip);
                    if (Mouse.IsOver(rect2))
                    {
                        Widgets.DrawHighlight(rect2);
                    }
                    num += rect2.height + 2f;
                    Rect rect3 = new Rect(20f, num, rect.width - 20f, 24f);
                    Widgets.Label(rect3, "RomanceFactor".Translate() + ": " + (comp.RomanceFactor * 10));
                    TipSignal RomanceFactorTooltip = "RomanceFactorTooltip".Translate();
                    TooltipHandler.TipRegion(rect3, RomanceFactorTooltip);
                    if (Mouse.IsOver(rect3))
                    {
                        Widgets.DrawHighlight(rect3);
                    }
                    num += rect3.height + 2f;
                }
                Rect rect4 = new Rect(20f, num, rect.width - 20f, 24f);
                Widgets.Label(rect4, "BodyWeight".Translate() + ": " + ((comp.BodyWeight + 70) * pawn.BodySize) + " kg (" + pawn.story.bodyType + ")");
                TipSignal BodyWeightTooltip = "BodyWeightTooltip".Translate();
                TooltipHandler.TipRegion(rect4, BodyWeightTooltip);
                if (Mouse.IsOver(rect4))
                {
                    Widgets.DrawHighlight(rect4);
                }
                num += rect4.height + 2f;
                Rect rect5 = new Rect(20f, num, rect.width - 20f, 24f);
                Widgets.Label(rect5, "PsychicFactor".Translate() + ": " + comp.PsychicFactor.ToStringByStyle(ToStringStyle.PercentZero, ToStringNumberSense.Offset));
                TipSignal PsychicFactorTooltip = "PsychicFactorTooltip".Translate();
                TooltipHandler.TipRegion(rect5, PsychicFactorTooltip);
                if (Mouse.IsOver(rect5))
                {
                    Widgets.DrawHighlight(rect5);
                }
                //GUI.EndGroup();
            }
        }
    }
}
