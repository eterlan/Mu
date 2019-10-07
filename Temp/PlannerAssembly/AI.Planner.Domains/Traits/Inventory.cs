using System;
using Unity.Entities;
using Unity.AI.Planner;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using AI.Planner.Domains.Enums;

namespace AI.Planner.Domains
{
    [Serializable]
    public struct Inventory : ITrait, IEquatable<Inventory>
    {
        public const bool IsZeroSized = false;
        public AI.Planner.Domains.Enums.ItemType ItemType;
        public AI.Planner.Domains.Enums.ConsumableType ConsumableType;
        public System.Int64 Restoration;
        public AI.Planner.Domains.Enums.ResourceType ResourceType;
        public System.Int64 Amount;
        public System.Int64 CapacityPerGrid;
        public AI.Planner.Domains.Enums.NeedType SatisfiesNeed;

        public void SetField(string fieldName, object value)
        {
            switch (fieldName)
            {
                case nameof(ItemType):
                    ItemType = (AI.Planner.Domains.Enums.ItemType)Enum.ToObject(typeof(AI.Planner.Domains.Enums.ItemType), value);
                    break;
                case nameof(ConsumableType):
                    ConsumableType = (AI.Planner.Domains.Enums.ConsumableType)Enum.ToObject(typeof(AI.Planner.Domains.Enums.ConsumableType), value);
                    break;
                case nameof(Restoration):
                    Restoration = (System.Int64)value;
                    break;
                case nameof(ResourceType):
                    ResourceType = (AI.Planner.Domains.Enums.ResourceType)Enum.ToObject(typeof(AI.Planner.Domains.Enums.ResourceType), value);
                    break;
                case nameof(Amount):
                    Amount = (System.Int64)value;
                    break;
                case nameof(CapacityPerGrid):
                    CapacityPerGrid = (System.Int64)value;
                    break;
                case nameof(SatisfiesNeed):
                    SatisfiesNeed = (AI.Planner.Domains.Enums.NeedType)Enum.ToObject(typeof(AI.Planner.Domains.Enums.NeedType), value);
                    break;
            }
        }

        public bool Equals(Inventory other)
        {
            return ItemType == other.ItemType && ConsumableType == other.ConsumableType && Restoration == other.Restoration && ResourceType == other.ResourceType && Amount == other.Amount && CapacityPerGrid == other.CapacityPerGrid && SatisfiesNeed == other.SatisfiesNeed;
        }

        public override int GetHashCode()
        {
            return 397
                ^ ItemType.GetHashCode()
                ^ ConsumableType.GetHashCode()
                ^ Restoration.GetHashCode()
                ^ ResourceType.GetHashCode()
                ^ Amount.GetHashCode()
                ^ CapacityPerGrid.GetHashCode()
                ^ SatisfiesNeed.GetHashCode();
        }

        public override string ToString()
        {
            return $"Inventory: {ItemType} {ConsumableType} {Restoration} {ResourceType} {Amount} {CapacityPerGrid} {SatisfiesNeed}";
        }
    }
}
