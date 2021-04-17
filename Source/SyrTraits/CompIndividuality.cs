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
            if (parent is Pawn pawn)
            {
                ReplaceVanillaTraits();
                //ReplaceOldTraits();
            }
            IndividualityValueSetup();
        }

        public override void ReceiveCompSignal(string signal)
        {
            if (signal == "bodyTypeSelected" && parent is Pawn pawn)
            {
                BodyWeight = RandomBodyWeightByBodyType(pawn);
            }
            if (signal == "traitsGenerated")
            {
                ReplaceVanillaTraits();
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            IndividualityValueSetup();
            ReplaceVanillaTraits();
            //ReplaceOldTraits();
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
                if (sexuality == Sexuality.Undefined)
                {
                    sexuality = Sexuality.Asexual;
                }
                if (RomanceFactor == -1f)
                {
                    RomanceFactor = 0f;
                }
                if (PsychicFactor == -2f)
                {
                    PsychicFactor = RandomPsychicFactor();
                }
            }
            else if (parent.def.defName == "Harpy")
            {
                if (sexuality == Sexuality.Undefined)
                {
                    sexuality = Sexuality.Bisexual;
                }
            }
            if (sexuality == Sexuality.Undefined)
            {
                sexuality = RandomSexualityByWeight();
            }
            if (RomanceFactor == -1f)
            {
                RomanceFactor = GenMath.RoundTo(Rand.Range(0.1f, 1f), 0.1f);
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

        public float RandomPsychicFactor()
        {
            float num = Mathf.Clamp(Rand.Gaussian(0f, 0.5f), -1f, 1f);
            if (num > -0.3f && num < 0.3) //this ensures there is few with 20% and many with 0% psychicFactor
            {
                num = 0f;
            }
            return GenMath.RoundTo(num, 0.2f);
        }

        public int RandomBodyWeightByBodyType(Pawn pawn)
        {
            int num;
            if (pawn?.story?.bodyType != null)
            {
                if (parent.def.defName == "Harpy")
                {
                    num = GenMath.RoundTo(Rand.Range(0, 10), 10);
                }
                else if (pawn.story.bodyType == BodyTypeDefOf.Fat)
                {
                    num = GenMath.RoundTo(Rand.Range(30, 40), 10);
                }
                else if (pawn.story.bodyType == BodyTypeDefOf.Hulk)
                {
                    num = GenMath.RoundTo(Rand.Range(10, 20), 10);
                }
                else if (pawn.story.bodyType == BodyTypeDefOf.Thin)
                {
                    num = GenMath.RoundTo(Rand.Range(-20, -10), 10);
                }
                else if (pawn.story.bodyType == BodyTypeDefOf.Female)
                {
                    num = GenMath.RoundTo(Rand.Range(-10, 0), 10);
                }
                else
                {
                    num = GenMath.RoundTo(Rand.Range(0, 10), 10);
                }
            }
            else
            {
                return -29;
            }
            return Mathf.Clamp(num, -20, 40);
        }

        public Sexuality RandomSexualityByWeight()
        {
            return SexualityArray.RandomElementByWeight(x => Probability(x));
        }
        Sexuality[] SexualityArray = { Sexuality.Straight, Sexuality.Bisexual, Sexuality.Gay, Sexuality.Asexual };

        public float Probability(Sexuality val)
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
       
        public void ReplaceVanillaTraits()
        {
            if (parent is Pawn pawn)
            {
                CompIndividuality comp = pawn.TryGetComp<CompIndividuality>();
                if (comp != null && pawn?.story?.traits != null && (pawn.story.traits.HasTrait(TraitDefOf.Gay) || pawn.story.traits.HasTrait(TraitDefOf.Bisexual) || pawn.story.traits.HasTrait(TraitDefOf.Asexual) || pawn.story.traits.HasTrait(TraitDefOf.PsychicSensitivity)))
                {
                    if (pawn.story.traits.HasTrait(TraitDefOf.Gay))
                    {
                        comp.sexuality = Sexuality.Gay;
                    }
                    else if (pawn.story.traits.HasTrait(TraitDefOf.Bisexual))
                    {
                        comp.sexuality = Sexuality.Bisexual;
                    }
                    else if (pawn.story.traits.HasTrait(TraitDefOf.Asexual))
                    {
                        comp.sexuality = Sexuality.Asexual;
                    }
                    if (pawn.story.traits.HasTrait(TraitDefOf.PsychicSensitivity))
                    {
                        switch (pawn.story.traits.GetTrait(TraitDefOf.PsychicSensitivity).Degree)
                        {
                            case -2: comp.PsychicFactor = -1f; break;
                            case -1: comp.PsychicFactor = -0.4f; break;
                            case 1: comp.PsychicFactor = 0.4f; break;
                            case 2: comp.PsychicFactor = 0.8f; break;
                        }

                    }
                    if (!SyrIndividuality.RomanceDisabled)
                    {
                        pawn.story.traits.allTraits.RemoveAll(t => t.def == TraitDefOf.Bisexual || t.def == TraitDefOf.Asexual || t.def == TraitDefOf.Gay || t.def == TraitDefOf.PsychicSensitivity);
                        IEnumerable<TraitDef> allTraitDefs = DefDatabase<TraitDef>.AllDefsListForReading;
                        Func<TraitDef, float> weightSelector = (TraitDef tr) => tr.GetGenderSpecificCommonality(pawn.gender);
                        TraitDef newTraitDef = allTraitDefs.RandomElementByWeight(weightSelector);
                        if (!pawn.story.traits.HasTrait(newTraitDef) && (pawn.Faction == null || Faction.OfPlayerSilentFail == null || !pawn.Faction.HostileTo(Faction.OfPlayer) || newTraitDef.allowOnHostileSpawn)
                            && !pawn.story.traits.allTraits.Any((Trait tr) => newTraitDef.ConflictsWith(tr)) && (newTraitDef.requiredWorkTypes == null
                            || !pawn.OneOfWorkTypesIsDisabled(newTraitDef.requiredWorkTypes)) && !pawn.WorkTagIsDisabled(newTraitDef.requiredWorkTags))
                        {
                            int degree = PawnGenerator.RandomTraitDegree(newTraitDef);
                            if (pawn.story.childhood == null || !pawn.story.childhood.DisallowsTrait(newTraitDef, degree) && (pawn.story.adulthood == null || !pawn.story.adulthood.DisallowsTrait(newTraitDef, degree)))
                            {
                                Trait trait = new Trait(newTraitDef, degree, false);
                                pawn.story.traits.GainTrait(trait);
                            }
                        }
                    }
                }
            }
        }

        //compatibility with older versions
        /*public void ReplaceOldTraits()
        {
            Pawn pawn = parent as Pawn;
            if (pawn?.story?.traits != null) 
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
        }*/
    }
}
