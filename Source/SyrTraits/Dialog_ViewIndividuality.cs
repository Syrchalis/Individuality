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

        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(350f, 300f);
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            soundClose = SoundDefOf.InfoCard_Close;
            closeOnClickedOutside = true;
            absorbInputAroundWindow = false;
            forcePause = true;
            preventCameraMotion = false;
            doCloseX = true;
            closeOnAccept = true;
            closeOnCancel = true;
            Rect mainRect = new Rect(inRect.x -10f, inRect.y, inRect.width +10f, inRect.height);
            if (Find.WindowStack.IsOpen(typeof(Dialog_Trade)) || Current.ProgramState != ProgramState.Playing)
            {
                IndividualityCardUtility.DrawIndividualityCard(mainRect, pawn);
            }
            else
            {
                IndividualityCardUtility.DrawIndividualityCard(mainRect, Find.Selector.SingleSelectedThing as Pawn);
            }
        }
    }
}
