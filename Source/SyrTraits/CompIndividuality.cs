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
    public class CompIndividuality : ThingComp
    {
        public bool disabled = false;
        public int BodyWeight= -29;
        public float PsychicFactor = -2f;
        public float RomanceFactor = -1f;
        public Sexuality sexuality = Sexuality.Undefined;
        public enum Sexuality : byte
        {
            Undefined,
            Straight,
            Bisexual,
            Gay,
            Asexual
        }
        
        public CompIndividuality()
        {

        }

        public CompProperties_Individuality Props
        {
            get
            {
                return (CompProperties_Individuality)props;
            }
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            Pawn pawn = parent as Pawn;
            if (parent.def.defName == "ChjDroid")
            {
                disabled = true;
                RomanceFactor = 0f;
                BodyWeight = 0;
                sexuality = Sexuality.Asexual;
            }
            else if (parent.def.defName == "Harpy")
            {
                if (pawn != null && pawn.gender == Gender.Female)
                {
                    sexuality = Sexuality.Bisexual;
                }
            }
            IndividualityValueSetup();
        }

        //Adding to saves and sanity check
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            Pawn pawn = parent as Pawn;
            if (parent.def.defName == "ChjDroid")
            {
                disabled = true;
                RomanceFactor = 0f;
                BodyWeight = 0;
                sexuality = Sexuality.Asexual;
            }
            else if (parent.def.defName == "Harpy")
            {
                if (pawn != null && pawn.gender == Gender.Female)
                {
                    sexuality = Sexuality.Bisexual;
                }
            }
            IndividualityValueSetup();
            if (pawn?.story?.traits != null) //compatibility with older versions
            {
                TraitSet traitset = pawn.story.traits;
                if (traitset.HasTrait(SyrTraitDefOf.SYR_Blacksmith))
                {
                    traitset.allTraits.RemoveAll((Trait t) => t.def == SyrTraitDefOf.SYR_Blacksmith);
                    traitset.GainTrait(new Trait(SyrTraitDefOf.SYR_KeenEye, 0, false));
                }
                if (traitset.HasTrait(SyrTraitDefOf.SYR_Scientist))
                {
                    traitset.allTraits.RemoveAll((Trait t) => t.def == SyrTraitDefOf.SYR_Scientist);
                    traitset.GainTrait(new Trait(SyrTraitDefOf.SYR_CreativeThinker, 0, false));
                }
                if (traitset.HasTrait(SyrTraitDefOf.SYR_Tinkerer))
                {
                    traitset.allTraits.RemoveAll((Trait t) => t.def == SyrTraitDefOf.SYR_Tinkerer);
                    traitset.GainTrait(new Trait(SyrTraitDefOf.SYR_MechanoidExpert, 0, false));
                }
                if (traitset.HasTrait(SyrTraitDefOf.SYR_Chef))
                {
                    traitset.allTraits.RemoveAll((Trait t) => t.def == SyrTraitDefOf.SYR_Chef);
                    traitset.GainTrait(new Trait(SyrTraitDefOf.Gourmand, 0, false));
                }
                if (traitset.HasTrait(SyrTraitDefOf.SYR_MedicalTraining))
                {
                    traitset.allTraits.RemoveAll((Trait t) => t.def == SyrTraitDefOf.SYR_MedicalTraining);
                    traitset.GainTrait(new Trait(SyrTraitDefOf.SYR_SteadyHands, 0, false));
                }
                if (traitset.HasTrait(SyrTraitDefOf.SYR_Hotblooded))
                {
                    traitset.allTraits.RemoveAll((Trait t) => t.def == SyrTraitDefOf.SYR_Hotblooded);
                    traitset.GainTrait(new Trait(SyrTraitDefOf.SYR_HandEyeCoordination, 0, false));
                }
            }
        }

        public override void ReceiveCompSignal(string signal)
        {
            if (signal == "bodyTypeSelected")
            {
                IndividualityValueSetup();
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref sexuality, "Individuality_Sexuality", Sexuality.Undefined, false);
            Scribe_Values.Look(ref BodyWeight, "Individuality_BodyWeight", -29, false);
            Scribe_Values.Look(ref RomanceFactor, "Individuality_RomanceFactor", -1f, false);
            Scribe_Values.Look(ref PsychicFactor, "Individuality_PsychicFactor", -2f, false);
        }

        public void IndividualityValueSetup()
        {
            Pawn pawn = parent as Pawn;
            if (parent.def.defName == "ChjDroid")
            {
                disabled = true;
                RomanceFactor = 0f;
                BodyWeight = 0;
                sexuality = Sexuality.Asexual;
            }
            else if (parent.def.defName == "Harpy")
            {
                if (pawn != null && pawn.gender == Gender.Female)
                {
                    sexuality = Sexuality.Bisexual;
                }
            }
            if (!disabled)
            {
                if (!SyrIndividuality.PsychologyIsActive)
                {
                    if (sexuality == Sexuality.Undefined)
                    {
                        sexuality = RandomSexualityByWeight();
                    }
                    if (RomanceFactor == -1f)
                    {
                        RomanceFactor = GenMath.RoundTo(Rand.Range(0.1f, 1f), 0.1f);
                    }
                }
                if (PsychicFactor == -2f)
                {
                    PsychicFactor = RandomPsychicFactor();
                }
                if (pawn != null && BodyWeight == -29)
                {
                    BodyWeight = RandomBodyWeightByBodyType(pawn);
                }
            }
        }

        Sexuality[] SexualityArray = { Sexuality.Straight, Sexuality.Bisexual, Sexuality.Gay, Sexuality.Asexual };
        float Probability(Sexuality val)
        {
            switch (val)
            {
                case Sexuality.Straight: return SyrIndividualitySettings.commonalityStraight;
                case Sexuality.Bisexual: return SyrIndividualitySettings.commonalityBi;
                case Sexuality.Gay: return SyrIndividualitySettings.commonalityGay;
                case Sexuality.Asexual: return SyrIndividualitySettings.commonalityAsexual;
            }
            return 0;
        }

        Sexuality RandomSexualityByWeight()
        {
            return SexualityArray.RandomElementByWeight(x => Probability(x));
        }

        float RandomPsychicFactor()
        {
            float num = Mathf.Clamp(Rand.Gaussian(0f, 0.5f), -1f, 1f);
            if (num > -0.3f && num < 0.3) //this ensures there is few with 20% and many with 0% psychicFactor
            {
                num = 0f;
            }
            return GenMath.RoundTo(num, 0.2f);
        }

        int RandomBodyWeightByBodyType(Pawn pawn)
        {
            int num;
            if (pawn?.story?.bodyType != null)
            {
                if (pawn.story.bodyType == BodyTypeDefOf.Fat)
                    num = GenMath.RoundTo(Rand.Range(30, 40), 10);
                else if (pawn.story.bodyType == BodyTypeDefOf.Hulk)
                    num = GenMath.RoundTo(Rand.Range(10, 20), 10);
                else if (pawn.story.bodyType == BodyTypeDefOf.Thin)
                    num = GenMath.RoundTo(Rand.Range(-20, -10), 10);
                else if (pawn.story.bodyType == BodyTypeDefOf.Female)
                    num = GenMath.RoundTo(Rand.Range(-10, 0), 10);
                else
                    num = GenMath.RoundTo(Rand.Range(0, 10), 10);
            }
            else
            {
                return -29;
            }
            return Mathf.Clamp(num, -20, 40);
        }
    }
}
