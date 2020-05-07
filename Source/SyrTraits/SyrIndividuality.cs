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
    public class SyrIndividuality : Mod
    {
        public static SyrIndividualitySettings settings;

        public SyrIndividuality(ModContentPack content) : base(content)
        {
            settings = GetSettings<SyrIndividualitySettings>();
            //CharacterCardUtility.BasePawnCardSize.y = 487f;
        }

        public override string SettingsCategory() => "SyrTraitsSettingsCategory".Translate();
        public static bool RationalRomanceActive => ModsConfig.ActiveModsInLoadOrder.Any(m => m.Name.Contains("Rational Romance"));
        public static bool PsychologyActive => ModsConfig.ActiveModsInLoadOrder.Any(m => m.Name.Contains("Psychology"));
        public static bool RomanceDisabled => PsychologyActive || RationalRomanceActive || SyrIndividualitySettings.disableRomance;

        public override void DoSettingsWindowContents(Rect inRect)
        {
            checked
            {
                float sum = Mathf.Max((SyrIndividualitySettings.commonalityStraight + SyrIndividualitySettings.commonalityBi + SyrIndividualitySettings.commonalityGay + SyrIndividualitySettings.commonalityAsexual), 0.05f);
                Listing_Standard listing_Standard = new Listing_Standard();
                listing_Standard.Begin(inRect);
                listing_Standard.Label("SyrTraitsTraitCount".Translate());
                listing_Standard.IntRange(ref SyrIndividualitySettings.traitCount, 0, 8);
                listing_Standard.Gap(24f);
                if (PsychologyActive || RationalRomanceActive)
                {
                    GUI.color = Color.red;
                    string disabledReason = "";
                    if (PsychologyActive) disabledReason = "SyrTraitsDisabledPsychology".Translate();
                    else if (RationalRomanceActive) disabledReason = "SyrTraitsDisabledRationalRomance".Translate();
                    listing_Standard.Label("SyrTraitsDisabled".Translate() + ": " + disabledReason);
                    GUI.color = Color.white;
                }
                else
                {
                    listing_Standard.CheckboxLabeled("SyrTraitsDisableRomance".Translate(), ref SyrIndividualitySettings.disableRomance, "SyrTraitsDisableRomanceTooltip".Translate());
                }
                listing_Standard.Gap(24f);
                if (!RomanceDisabled)
                {
                    listing_Standard.Label("SyrTraitsSexualityCommonality".Translate());
                    listing_Standard.Label("SyrTraitsSexualityCommonalityStraight".Translate() + ": " + (SyrIndividualitySettings.commonalityStraight / sum).ToStringByStyle(ToStringStyle.PercentZero));
                    SyrIndividualitySettings.commonalityStraight = listing_Standard.Slider(GenMath.RoundTo(SyrIndividualitySettings.commonalityStraight, 0.1f), 0f, 1f);
                    listing_Standard.Label("SyrTraitsSexualityCommonalityBi".Translate() + ": " + (SyrIndividualitySettings.commonalityBi / sum).ToStringByStyle(ToStringStyle.PercentZero));
                    SyrIndividualitySettings.commonalityBi = listing_Standard.Slider(GenMath.RoundTo(SyrIndividualitySettings.commonalityBi, 0.1f), 0f, 1f);
                    listing_Standard.Label("SyrTraitsSexualityCommonalityGay".Translate() + ": " + (SyrIndividualitySettings.commonalityGay / sum).ToStringByStyle(ToStringStyle.PercentZero));
                    SyrIndividualitySettings.commonalityGay = listing_Standard.Slider(GenMath.RoundTo(SyrIndividualitySettings.commonalityGay, 0.1f), 0f, 1f);
                    listing_Standard.Label("SyrTraitsSexualityCommonalityAsexual".Translate() + ": " + (SyrIndividualitySettings.commonalityAsexual / sum).ToStringByStyle(ToStringStyle.PercentZero));
                    SyrIndividualitySettings.commonalityAsexual = listing_Standard.Slider(GenMath.RoundTo(SyrIndividualitySettings.commonalityAsexual, 0.1f), 0f, 1f);
                    listing_Standard.Gap(12f);
                }
                if (listing_Standard.ButtonText("SyrTraitsDefaultSettings".Translate(), "SyrTraitsDefaultSettingsTooltip".Translate()))
                {
                    SyrIndividualitySettings.traitCount.min = 2;
                    SyrIndividualitySettings.traitCount.max = 3;
                    SyrIndividualitySettings.commonalityStraight = 0.8f;
                    SyrIndividualitySettings.commonalityBi = 0.1f;
                    SyrIndividualitySettings.commonalityGay = 0.1f;
                    SyrIndividualitySettings.commonalityAsexual = 0f;
                }
                listing_Standard.End();
                settings.Write();
            }
        }
        public override void WriteSettings()
        {
            base.WriteSettings();
        }
    }
}
