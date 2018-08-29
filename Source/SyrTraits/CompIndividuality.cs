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
            if (sexuality == Sexuality.None)
            {
                sexuality = RandomSexualityByWeight();
            }
            if (sexuality == Sexuality.None)
            {
                sexuality = RandomSexualityByWeight();
            }
            if (RomanceFactor == -999f)
            {
                RomanceFactor = GenMath.RoundTo(Rand.Range(0.25f, 1f), 0.1f);
            }
            if (PsychicFactor < -999f)
            {
                PsychicFactor = GenMath.RoundTo(Rand.Range(-1f, 1f), 0.1f);
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
            if (BodyWeight == -999)
            {
                BodyWeight = Rand.Range(-10, 10);
            }

        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref sexuality, "sexuality", Sexuality.None, false);
            Scribe_Values.Look(ref BodyWeight, "bodyWeight", -999, false);
            Scribe_Values.Look(ref RomanceFactor, "romanceFactor", -999f, false);
            Scribe_Values.Look(ref PsychicFactor, "psychicFactor", -999f, false);
            if (sexuality == Sexuality.None)
            {
                sexuality = RandomSexualityByWeight();
            }
            if (RomanceFactor == -999f)
            {
                RomanceFactor = GenMath.RoundTo(Rand.Range(0.25f, 1f), 0.1f);
            }
            if (PsychicFactor < -999f)
            {
                PsychicFactor = GenMath.RoundTo(Rand.Range(-1f, 1f), 0.1f);
            }
            if (BodyWeight == -999)
            {
                BodyWeight = Rand.Range(-10, 10);
            }
        }

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
