using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using RimWorld;
using Verse;
using UnityEngine;
using Verse.Sound;

namespace SyrTraits
{
    public static class IndividualityCardUtility
    {
        public static bool editMode = false;
        public static void DrawIndividualityCard(Rect rect, Pawn pawn)
        {
            CompIndividuality comp = pawn.TryGetComp<CompIndividuality>();
            if (pawn != null && comp != null)
            {
                Rect iconRect = new Rect(0f, 10f, 40f, 40f);
                Widgets.ThingIcon(iconRect, pawn, 1f);
                Text.Font = GameFont.Medium;
                Rect nameRect = new Rect(56f, 0f, rect.width, 30f);
                Widgets.Label(nameRect, pawn.Name.ToStringShort);
                Text.Font = GameFont.Tiny;
                float num = nameRect.y + nameRect.height;
                Rect titleRect = new Rect(56f, num, rect.width, 24f);
                Widgets.Label(titleRect, "IndividualityWindow".Translate());
                Text.Font = GameFont.Small;
                num += titleRect.height + 17f;
                if (!SyrIndividuality.RomanceDisabled)
                {
                    Rect rect2 = new Rect(0f, num, rect.width - 10f, 24f);
                    Widgets.Label(new Rect(10f, num, rect.width, 24f), "SexualityPawn".Translate() + ": " + comp.sexuality);
                    TipSignal SexualityPawnTooltip = "SexualityPawnTooltip".Translate();
                    TooltipHandler.TipRegion(rect2, SexualityPawnTooltip);
                    Widgets.DrawHighlightIfMouseover(rect2);
                    num += rect2.height + 2f;
                    Rect rect3 = new Rect(0f, num, rect.width - 10f, 24f);
                    Widgets.Label(new Rect(10f, num, rect.width, 24f), "RomanceFactor".Translate() + ": " + (comp.RomanceFactor * 2).ToStringByStyle(ToStringStyle.PercentZero));
                    TipSignal RomanceFactorTooltip = "RomanceFactorTooltip".Translate();
                    TooltipHandler.TipRegion(rect3, RomanceFactorTooltip);
                    Widgets.DrawHighlightIfMouseover(rect3);
                    num += rect3.height + 2f;
                    if (editMode)
                    {
                        if (ScrolledDown(rect2, true) || LeftClicked(rect2))
                        {
                            SoundDefOf.DragSlider.PlayOneShotOnCamera(null);
                            comp.sexuality += 1;
                            if (comp.sexuality > CompIndividuality.Sexuality.Asexual)
                            {
                                comp.sexuality = CompIndividuality.Sexuality.Straight;
                            }
                        }
                        else if (ScrolledUp(rect2, true) || RightClicked(rect2))
                        {
                            SoundDefOf.DragSlider.PlayOneShotOnCamera(null);
                            comp.sexuality -= 1;
                            if (comp.sexuality < CompIndividuality.Sexuality.Straight)
                            {
                                comp.sexuality = CompIndividuality.Sexuality.Asexual;
                            }
                        }
                        else if (ScrolledDown(rect3, true) || LeftClicked(rect3))
                        {
                            SoundDefOf.DragSlider.PlayOneShotOnCamera(null);
                            comp.RomanceFactor += 0.1f;
                            if (comp.RomanceFactor > 1.05f)
                            {
                                comp.RomanceFactor = 0.1f;
                            }
                        }
                        else if (ScrolledUp(rect3, true) || RightClicked(rect3))
                        {
                            SoundDefOf.DragSlider.PlayOneShotOnCamera(null);
                            comp.RomanceFactor -= 0.1f;
                            if (comp.RomanceFactor < 0.05f)
                            {
                                comp.RomanceFactor = 1f;
                            }
                        }
                    }
                }
                Rect rect4 = new Rect(0f, num, rect.width - 10f, 24f);
                Widgets.Label(new Rect(10f, num, rect.width, 24f), "PsychicFactor".Translate() + ": " + comp.PsychicFactor.ToStringByStyle(ToStringStyle.PercentZero, ToStringNumberSense.Offset));
                TipSignal PsychicFactorTooltip = "PsychicFactorTooltip".Translate();
                TooltipHandler.TipRegion(rect4, PsychicFactorTooltip);
                Widgets.DrawHighlightIfMouseover(rect4);
                num += rect4.height + 2f;
                Rect rect5 = new Rect(0f, num, rect.width - 10f, 24f);
                Widgets.Label(new Rect(10f, num, rect.width, 24f), "BodyWeight".Translate() + ": " + ((comp.BodyWeight + 70) * pawn.BodySize) + " kg (" + pawn.story.bodyType + ")");
                TipSignal BodyWeightTooltip = "BodyWeightTooltip".Translate();
                TooltipHandler.TipRegion(rect5, BodyWeightTooltip);
                Widgets.DrawHighlightIfMouseover(rect5);
                num += rect5.height + 7f;
                Rect editModeTooltip = new Rect(10f, num, rect.width, 24f);
                if (editMode)
                {
                    GUI.color = Color.red;
                    Text.Font = GameFont.Tiny;
                    Widgets.Label(editModeTooltip, "SyrIndividuality_EditModeTooltip".Translate());
                    GUI.color = Color.white;
                    Text.Font = GameFont.Small;
                }
                num += editModeTooltip.height + 13f;
                Rect checkBoxRect = new Rect(rect.width / 2 - 90f, num, 180f, 40f);
                bool leftClick = Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Mouse0;
                bool rightClick = Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Mouse1;
                if (!editMode)
                {
                    if (Widgets.ButtonText(checkBoxRect, "SyrIndividuality_EditModeOn".Translate()))
                    {
                        SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
                        editMode = true;
                    }
                }
                else
                {
                    if (Widgets.ButtonText(checkBoxRect, "SyrIndividuality_EditModeOff".Translate()))
                    {
                        SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
                        editMode = false;
                    }
                    if (ScrolledDown(rect4, true) || LeftClicked(rect4))
                    {
                        SoundDefOf.DragSlider.PlayOneShotOnCamera(null);
                        comp.PsychicFactor += 0.2f;
                        if (comp.PsychicFactor > 1f)
                        {
                            comp.PsychicFactor = -1f;
                        }
                    }
                    else if (ScrolledUp(rect4, true) || RightClicked(rect4))
                    {
                        SoundDefOf.DragSlider.PlayOneShotOnCamera(null);
                        comp.PsychicFactor -= 0.2f;
                        if (comp.PsychicFactor < -1f)
                        {
                            comp.PsychicFactor = 1f;
                        }
                    }
                    else if (ScrolledDown(rect5, true) || LeftClicked(rect5))
                    {
                        SoundDefOf.DragSlider.PlayOneShotOnCamera(null);
                        comp.BodyWeight += 10;
                        if (comp.BodyWeight > 40)
                        {
                            comp.BodyWeight = -20;
                        }
                    }
                    else if (ScrolledUp(rect5, true) || RightClicked(rect5))
                    {
                        SoundDefOf.DragSlider.PlayOneShotOnCamera(null);
                        comp.BodyWeight -= 10;
                        if (comp.BodyWeight < -20)
                        {
                            comp.BodyWeight = 40;
                        }
                    }
                }
            }
        }

        public static bool Scrolled(Rect rect, ScrollDirection direction, bool stopPropagation)
        {
            bool flag = Event.current.type == EventType.ScrollWheel && ((Event.current.delta.y > 0f && direction == ScrollDirection.Up) || (Event.current.delta.y < 0f && direction == ScrollDirection.Down)) && Mouse.IsOver(rect);
            if (flag && stopPropagation)
            {
                Event.current.Use();
            }
            return flag;
        }
        public static bool ScrolledUp(Rect rect, bool stopPropagation = false)
        {
            return Scrolled(rect, ScrollDirection.Up, stopPropagation);
        }
        public static bool ScrolledDown(Rect rect, bool stopPropagation = false)
        {
            return Scrolled(rect, ScrollDirection.Down, stopPropagation);
        }
        public enum ScrollDirection
        {
            Up,
            Down
        }

        public static bool Clicked(Rect rect, int button = 0)
        {
            return Event.current.type == EventType.MouseDown && Event.current.button == button && Mouse.IsOver(rect);
        }
        public static bool LeftClicked(Rect rect)
        {
            return Clicked(rect, 0);
        }
        public static bool RightClicked(Rect rect)
        {
            return Clicked(rect, 1);
        }
    }
}
