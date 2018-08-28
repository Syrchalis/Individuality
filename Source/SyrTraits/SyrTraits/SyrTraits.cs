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
    public class SyrTraits : Mod
    {
        public static SyrTraitsSettings settings;

        public SyrTraits(ModContentPack content) : base(content)
        {
            settings = GetSettings<SyrTraitsSettings>();
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
                listing_Standard.Label("SyrTraitsSexualityCommonality".Translate());
                listing_Standard.Label("SyrTraitsSexualityCommonalityStraight".Translate() + ": " + SyrTraitsSettings.commonalityStraight.ToStringByStyle(ToStringStyle.PercentZero));
                SyrTraitsSettings.commonalityStraight = listing_Standard.Slider(GenMath.RoundTo(SyrTraitsSettings.commonalityStraight, 0.05f), 0f, (1f - SyrTraitsSettings.commonalityBi - SyrTraitsSettings.commonalityGay));
                listing_Standard.Label("SyrTraitsSexualityCommonalityBi".Translate() + ": " + SyrTraitsSettings.commonalityBi.ToStringByStyle(ToStringStyle.PercentZero));
                SyrTraitsSettings.commonalityBi = listing_Standard.Slider(GenMath.RoundTo(SyrTraitsSettings.commonalityBi, 0.05f), 0f, (1f - SyrTraitsSettings.commonalityGay - SyrTraitsSettings.commonalityStraight));
                listing_Standard.Label("SyrTraitsSexualityCommonalityGay".Translate() + ": " + SyrTraitsSettings.commonalityGay.ToStringByStyle(ToStringStyle.PercentZero));
                SyrTraitsSettings.commonalityGay = listing_Standard.Slider(GenMath.RoundTo(SyrTraitsSettings.commonalityGay, 0.05f), 0f, (1f - SyrTraitsSettings.commonalityBi - SyrTraitsSettings.commonalityStraight));
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
