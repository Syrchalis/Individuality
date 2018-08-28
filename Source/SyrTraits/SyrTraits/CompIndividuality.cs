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
            RomanceFactor = GenMath.RoundTo(Rand.Range(0.25f, 1f), 0.1f);
            float num = Mathf.Clamp(Rand.Gaussian(-0.1f, 0.45f), -1f, 0.8f);
            if (num > -0.25f && num < 0.25) //this ensures there is few with 25% and many with 0% psychicFactor
            {
                num = 0f;
            }
            PsychicFactor = GenMath.RoundTo(num, 0.25f);
            BodyWeight = GenMath.RoundTo(Rand.Range(-10, 10), 5);
            Pawn pawn = parent as Pawn;
            if (pawn != null)
            {
                if (pawn.story.bodyType == BodyTypeDefOf.Fat)
                {
                    BodyWeight += GenMath.RoundTo(Rand.Range(20, 40), 5);
                }
                if (pawn.story.bodyType == BodyTypeDefOf.Hulk)
                {
                    BodyWeight += GenMath.RoundTo(Rand.Range(10, 20), 5);
                }
                if (pawn.story.bodyType == BodyTypeDefOf.Thin)
                {
                    BodyWeight += GenMath.RoundTo(Rand.Range(-20, -10), 5);
                }
                if (pawn.story.bodyType == BodyTypeDefOf.Female)
                {
                    BodyWeight += GenMath.RoundTo(Rand.Range(-10, 0), 5);
                }
                if (BodyWeight < -30)
                {
                    BodyWeight = -30;
                }
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
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            if (sexuality == Sexuality.None)
            {
                sexuality = RandomSexualityByWeight();
            }
            if (RomanceFactor == 0f)
            {
                RomanceFactor = GenMath.RoundTo(Rand.Range(0.25f, 1f), 0.1f);
            }
            if (PsychicFactor < -1f)
            {
                PsychicFactor = GenMath.RoundTo(Rand.Range(-1f, 1f), 0.1f);
            }
            if (BodyWeight == -999)
            {
                BodyWeight = Rand.Range(-10, 10);
            }
            Scribe_Values.Look(ref sexuality, "sexuality", Sexuality.None, false);
            Scribe_Values.Look(ref BodyWeight, "bodyWeight", -999, false);
            Scribe_Values.Look(ref RomanceFactor, "romanceFactor", 0f, false);
            Scribe_Values.Look(ref PsychicFactor, "psychicFactor", -2f, false);
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
                case Sexuality.Straight: return SyrTraitsSettings.commonalityStraight;
                case Sexuality.Bisexual: return SyrTraitsSettings.commonalityBi;
                case Sexuality.Gay: return SyrTraitsSettings.commonalityGay;
            }
            return 0;
        }

        Sexuality RandomSexualityByWeight()
        {
            return SexualityArray.RandomElementByWeight(x => Probability(x));
        }
    }
}
