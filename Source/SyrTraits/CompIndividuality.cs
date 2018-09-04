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
    public class CompIndividuality : ThingComp
    {
        public CompIndividuality()
        {
            RomanceFactor = GenMath.RoundTo(Rand.Range(0.1f, 1f), 0.1f);
            float num = Mathf.Clamp(Rand.Gaussian(-0.1f, 0.45f), -1f, 0.8f);
            if (num > -0.2f && num < 0.2) //this ensures there is few with 20% and many with 0% psychicFactor
            {
                num = 0f;
            }
            PsychicFactor = GenMath.RoundTo(num, 0.2f);
            sexuality = RandomSexualityByWeight();
        }

        public CompProperties_Individuality Props
        {
            get
            {
                return (CompProperties_Individuality)props;
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (parent.def.defName == "ChjDroid")
            {
                disabled = true;
                RomanceFactor = 0f;
                PsychicFactor = 0f;
                sexuality = Sexuality.None;
                BodyWeight = 0f;
            }
            if (!disabled)
            {
                if (sexuality == Sexuality.None)
                {
                    sexuality = RandomSexualityByWeight();
                }
                if (RomanceFactor == -1f)
                {
                    RomanceFactor = GenMath.RoundTo(Rand.Range(0.1f, 1f), 0.1f);
                }
                if (PsychicFactor < -2f)
                {
                    PsychicFactor = GenMath.RoundTo(Rand.Range(-1f, 0.8f), 0.2f);
                }
                Pawn pawn = parent as Pawn;
                if (pawn != null)
                {
                    if (pawn.story.bodyType == BodyTypeDefOf.Fat)
                        BodyWeight = GenMath.RoundTo(Rand.Range(20, 50), 5);
                    else if (pawn.story.bodyType == BodyTypeDefOf.Hulk)
                        BodyWeight = GenMath.RoundTo(Rand.Range(10, 30), 5);
                    else if (pawn.story.bodyType == BodyTypeDefOf.Thin)
                        BodyWeight = GenMath.RoundTo(Rand.Range(-30, -10), 5);
                    else if (pawn.story.bodyType == BodyTypeDefOf.Female)
                        BodyWeight = GenMath.RoundTo(Rand.Range(-10, 0), 5);
                    BodyWeight = Mathf.Clamp(BodyWeight, -30, 50);
                }
                if (BodyWeight == -39)
                {
                    BodyWeight = Rand.Range(-10, 10);
                }
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref sexuality, "Individuality_Sexuality", Sexuality.None, false);
            Scribe_Values.Look(ref BodyWeight, "Individuality_BodyWeight", -39, false);
            Scribe_Values.Look(ref RomanceFactor, "Individuality_RomanceFactor", -1f, false);
            Scribe_Values.Look(ref PsychicFactor, "Individuality_PsychicFactor", -2f, false);
            if (!disabled && !SyrIndividuality.PsychologyIsActive)
            {
                if (sexuality == Sexuality.None)
                {
                    sexuality = RandomSexualityByWeight();
                }
                if (RomanceFactor == -1f)
                {
                    RomanceFactor = GenMath.RoundTo(Rand.Range(0.1f, 1f), 0.1f);
                }
                if (PsychicFactor < -2f)
                {
                    PsychicFactor = GenMath.RoundTo(Rand.Range(-1f, 0.8f), 0.2f);
                }
                if (BodyWeight == -39)
                {
                    BodyWeight = Rand.Range(-10, 10);
                }
            }
        }

        public bool disabled = false;
        public float BodyWeight;
        public float PsychicFactor;
        public float RomanceFactor;
        public enum Sexuality : byte
        {
            None,
            Straight,
            Bisexual,
            Gay
        }
        public Sexuality sexuality;

        Sexuality[] SexualityArray = { Sexuality.Straight, Sexuality.Bisexual, Sexuality.Gay };
        float Probability(Sexuality val)
        {
            switch (val)
            {
                case Sexuality.Straight: return SyrIndividualitySettings.commonalityStraight;
                case Sexuality.Bisexual: return SyrIndividualitySettings.commonalityBi;
                case Sexuality.Gay: return SyrIndividualitySettings.commonalityGay;
            }
            return 0;
        }

        Sexuality RandomSexualityByWeight()
        {
            return SexualityArray.RandomElementByWeight(x => Probability(x));
        }
    }
}
