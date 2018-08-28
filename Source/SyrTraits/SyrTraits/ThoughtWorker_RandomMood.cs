using System;
using Verse;
using RimWorld;

namespace SyrTraits
{
    public class ThoughtWorker_RandomMood : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {

            switch ((p.GetHashCode() ^ (GenLocalDate.DayOfYear(p) + GenLocalDate.Year(p) + (int)(GenMath.RoundTo(GenLocalDate.DayPercent(p), 0.25f) * 10) * 60) * 391) % 12)
            {
                case 0:
                    return (p.story.traits.DegreeOfTrait(SyrTraitDefOf.Neurotic) == 2) ? ThoughtState.ActiveAtStage(0) : ThoughtState.Inactive;
                case 1:
                    return ThoughtState.ActiveAtStage(1);
                case 2:
                    return ThoughtState.ActiveAtStage(1);
                case 3:
                    return ThoughtState.ActiveAtStage(2);
                case 4:
                    return ThoughtState.ActiveAtStage(2);
                case 5:
                    return ThoughtState.Inactive;
                case 6:
                    return ThoughtState.Inactive;
                case 7:
                    return ThoughtState.ActiveAtStage(3);
                case 8:
                    return ThoughtState.ActiveAtStage(3);
                case 9:
                    return ThoughtState.ActiveAtStage(4);
                case 10:
                    return ThoughtState.ActiveAtStage(4);
                case 11:
                    return (p.story.traits.DegreeOfTrait(SyrTraitDefOf.Neurotic) == 2) ? ThoughtState.ActiveAtStage(5) : ThoughtState.Inactive;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}