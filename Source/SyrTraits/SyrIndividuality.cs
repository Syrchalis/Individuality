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
    public class SyrIndividuality : Mod
    {
        public static SyrIndividualitySettings settings;

        public SyrIndividuality(ModContentPack content) : base(content)
        {
            settings = GetSettings<SyrIndividualitySettings>();
            var harmony = HarmonyInstance.Create("Syrchalis.Rimworld.Traits");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            CharacterCardUtility.PawnCardSize.y = 487f;
        }

        public override string SettingsCategory() => "SyrTraitsSettingsCategory".Translate();
        public static bool PsychologyIsActive => ModsConfig.ActiveModsInLoadOrder.Any(m => m.Name == "Psychology");

        public override void DoSettingsWindowContents(Rect inRect)
        {
            checked
            {
                Listing_Standard listing_Standard = new Listing_Standard();
                listing_Standard.Begin(inRect);
                listing_Standard.Label("SyrTraitsTraitCount".Translate());
                listing_Standard.IntRange(ref SyrIndividualitySettings.traitCount, 0, 8);
                if (SyrIndividualitySettings.traitCount.max > 5 && !SyrIndividualitySettings.traitsTinyFont)
                {
                    GUI.color = Color.red;
                    listing_Standard.Label("SyrTraitsTraitCountWarning".Translate());
                    GUI.color = Color.white;
                }
                else
                {
                    listing_Standard.Gap(24f);
                }
                listing_Standard.CheckboxLabeled("SyrTraitsTraitsTinyFont".Translate(), ref SyrIndividualitySettings.traitsTinyFont, ("SyrTraitsTraitsTinyFontTooltip".Translate()));
                listing_Standard.Gap(12f);
                listing_Standard.CheckboxLabeled("SyrTraitsTraitsForcePassion".Translate(), ref SyrIndividualitySettings.traitsForcePassion, ("SyrTraitsTraitsForcePassionTooltip".Translate()));
                listing_Standard.Gap(12f);
                listing_Standard.Label("SyrTraitsSexualityCommonality".Translate());
                if (!PsychologyIsActive)
                {
                    listing_Standard.Label("SyrTraitsSexualityCommonalityStraight".Translate() + ": " + SyrIndividualitySettings.commonalityStraight.ToStringByStyle(ToStringStyle.PercentZero));
                    SyrIndividualitySettings.commonalityStraight = listing_Standard.Slider(GenMath.RoundTo(SyrIndividualitySettings.commonalityStraight, 0.05f), 0f, (1f - SyrIndividualitySettings.commonalityGay));
                    listing_Standard.Label("SyrTraitsSexualityCommonalityBi".Translate() + ": " + SyrIndividualitySettings.commonalityBi.ToStringByStyle(ToStringStyle.PercentZero), tooltip: "SyrTraitsSexualityCommonalityBiTooltip".Translate());
                    GUI.color = new Color(0.33f, 0.33f, 0.33f);
                    SyrIndividualitySettings.commonalityBi = listing_Standard.Slider(GenMath.RoundTo(1f - SyrIndividualitySettings.commonalityGay - SyrIndividualitySettings.commonalityStraight, 0.05f), 0f, 1f); // (1f - SyrIndividualitySettings.commonalityGay - SyrIndividualitySettings.commonalityStraight));
                    GUI.color = Color.white;
                    listing_Standard.Label("SyrTraitsSexualityCommonalityGay".Translate() + ": " + SyrIndividualitySettings.commonalityGay.ToStringByStyle(ToStringStyle.PercentZero));
                    SyrIndividualitySettings.commonalityGay = listing_Standard.Slider(GenMath.RoundTo(SyrIndividualitySettings.commonalityGay, 0.05f), 0f, (1f - SyrIndividualitySettings.commonalityStraight));
                }
                listing_Standard.Gap(12f);
                if (listing_Standard.ButtonText("SyrTraitsDefaultSettings".Translate(), "SyrTraitsDefaultSettingsTooltip".Translate()))
                {
                    SyrIndividualitySettings.traitCount.min = 2;
                    SyrIndividualitySettings.traitCount.max = 3;
                    SyrIndividualitySettings.traitsTinyFont = false;
                    SyrIndividualitySettings.traitsForcePassion = true;
                    SyrIndividualitySettings.commonalityStraight = 0.8f;
                    SyrIndividualitySettings.commonalityBi = 0.1f;
                    SyrIndividualitySettings.commonalityGay = 0.1f;
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
