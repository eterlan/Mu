using System;
using Unity.Entities;
using Unity.AI.Planner;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using AI.Planner.Domains.Enums;

namespace AI.Planner.Domains
{
    [Serializable]
    public struct WareHouse : ITrait, IEquatable<WareHouse>
    {
        public const bool IsZeroSized = false;
        public AI.Planner.Domains.Enums.ItemType ItemType;
        public AI.Planner.Domains.Enums.ConsumableType ConsumableType;
        public AI.Planner.Domains.Enums.ResourceType ResourceType;
        public System.Int64 Amount;
        public System.Int64 CapacityPerGrid;

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
                case nameof(ResourceType):
                    ResourceType = (AI.Planner.Domains.Enums.ResourceType)Enum.ToObject(typeof(AI.Planner.Domains.Enums.ResourceType), value);
                    break;
                case nameof(Amount):
                    Amount = (System.Int64)value;
                    break;
                case nameof(CapacityPerGrid):
                    CapacityPerGrid = (System.Int64)value;
                    break;
            }
        }

        public bool Equals(WareHouse other)
        {
            return ItemType == other.ItemType && ConsumableType == other.ConsumableType && ResourceType == other.ResourceType && Amount == other.Amount && CapacityPerGrid == other.CapacityPerGrid;
        }

        public override int GetHashCode()
        {
            return 397
                ^ ItemType.GetHashCode()
                ^ ConsumableType.GetHashCode()
                ^ ResourceType.GetHashCode()
                ^ Amount.GetHashCode()
                ^ CapacityPerGrid.GetHashCode();
        }

        public override string ToString()
        {
            return $"WareHouse: {ItemType} {ConsumableType} {ResourceType} {Amount} {CapacityPerGrid}";
        }
    }
}
