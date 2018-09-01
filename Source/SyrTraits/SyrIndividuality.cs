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
        }

        public override string SettingsCategory() => "SyrTraitsSettingsCategory".Translate();

        public override void DoSettingsWindowContents(Rect inRect)
        {
            checked
            {
                Listing_Standard listing_Standard = new Listing_Standard();
                listing_Standard.Begin(inRect);
                listing_Standard.Label("SyrTraitsTraitCount".Translate());
                listing_Standard.IntRange(ref SyrIndividualitySettings.traitCount, 0, 5);
                listing_Standard.Label("SyrTraitsSexualityCommonality".Translate());
                listing_Standard.Label("SyrTraitsSexualityCommonalityStraight".Translate() + ": " + SyrIndividualitySettings.commonalityStraight.ToStringByStyle(ToStringStyle.PercentZero));
                SyrIndividualitySettings.commonalityStraight = listing_Standard.Slider(GenMath.RoundTo(SyrIndividualitySettings.commonalityStraight, 0.05f), 0f, (1f - SyrIndividualitySettings.commonalityBi - SyrIndividualitySettings.commonalityGay));
                listing_Standard.Label("SyrTraitsSexualityCommonalityBi".Translate() + ": " + SyrIndividualitySettings.commonalityBi.ToStringByStyle(ToStringStyle.PercentZero));
                SyrIndividualitySettings.commonalityBi = listing_Standard.Slider(GenMath.RoundTo(SyrIndividualitySettings.commonalityBi, 0.05f), 0f, (1f - SyrIndividualitySettings.commonalityGay - SyrIndividualitySettings.commonalityStraight));
                listing_Standard.Label("SyrTraitsSexualityCommonalityGay".Translate() + ": " + SyrIndividualitySettings.commonalityGay.ToStringByStyle(ToStringStyle.PercentZero));
                SyrIndividualitySettings.commonalityGay = listing_Standard.Slider(GenMath.RoundTo(SyrIndividualitySettings.commonalityGay, 0.05f), 0f, (1f - SyrIndividualitySettings.commonalityBi - SyrIndividualitySettings.commonalityStraight));
                listing_Standard.CheckboxLabeled("SyrTraitsTraitsForcePassion".Translate(),ref SyrIndividualitySettings.traitsForcePassion ,("SyrTraitsTraitsForcePassionTooltip".Translate()));
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
