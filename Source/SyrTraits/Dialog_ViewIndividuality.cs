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
    public class Dialog_ViewIndividuality : Window
    {

        private Pawn pawn;

        public Dialog_ViewIndividuality(Pawn editFor)
        {
            pawn = editFor;
        }

        public override void DoWindowContents(Rect inRect)
        {
            bool flag = false;
            soundClose = SoundDefOf.InfoCard_Close;
            closeOnClickedOutside = true;
            absorbInputAroundWindow = false;
            forcePause = true;
            preventCameraMotion = false;
            if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.Escape))
            {
                flag = true;
                Event.current.Use();
            }
            Rect windowRect = inRect.ContractedBy(17f);
            Rect mainRect = new Rect(windowRect.x, windowRect.y, windowRect.width, windowRect.height - 20f);
            Rect okRect = new Rect(inRect.width / 3, mainRect.yMax + 10f, inRect.width / 3f, 30f);
            if (Current.ProgramState == ProgramState.Playing)
            {
                IndividualityCardUtility.DrawIndividualityCard(mainRect, Find.Selector.SingleSelectedThing as Pawn);
            }
            else
            {
                IndividualityCardUtility.DrawIndividualityCard(mainRect, pawn);
            }
            if (Widgets.ButtonText(okRect, "CloseButton".Translate(), true, false, true) || flag)
            {
                Close(true);
            }
            /*if (KeyBindingDefOf.NextColonist.KeyDownEvent)
            {
            }
            if (KeyBindingDefOf.PreviousColonist.KeyDownEvent)
            {
            }*/
        }
    }
}
