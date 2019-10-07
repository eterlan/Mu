using System;
using Unity.Entities;
using Unity.AI.Planner;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using AI.Planner.Domains.Enums;

namespace AI.Planner.Domains
{
    [Serializable]
    public struct ResourcesSpot : ITrait, IEquatable<ResourcesSpot>
    {
        public const bool IsZeroSized = false;
        public AI.Planner.Domains.Enums.ResourceType ResourceType;
        public System.Int64 Amount;

        public void SetField(string fieldName, object value)
        {
            switch (fieldName)
            {
                case nameof(ResourceType):
                    ResourceType = (AI.Planner.Domains.Enums.ResourceType)Enum.ToObject(typeof(AI.Planner.Domains.Enums.ResourceType), value);
                    break;
                case nameof(Amount):
                    Amount = (System.Int64)value;
                    break;
            }
        }

        public bool Equals(ResourcesSpot other)
        {
            return ResourceType == other.ResourceType && Amount == other.Amount;
        }

        public override int GetHashCode()
        {
            return 397
                ^ ResourceType.GetHashCode()
                ^ Amount.GetHashCode();
        }

        public override string ToString()
        {
            return $"ResourcesSpot: {ResourceType} {Amount}";
        }
    }
}
