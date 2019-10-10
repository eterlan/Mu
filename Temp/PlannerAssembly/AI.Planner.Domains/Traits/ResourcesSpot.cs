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
        public System.Int64 AverageAcquisition;
        public System.Single ExploitTime;

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
                case nameof(AverageAcquisition):
                    AverageAcquisition = (System.Int64)value;
                    break;
                case nameof(ExploitTime):
                    ExploitTime = (System.Single)value;
                    break;
            }
        }

        public bool Equals(ResourcesSpot other)
        {
            return ResourceType == other.ResourceType && Amount == other.Amount && AverageAcquisition == other.AverageAcquisition && ExploitTime == other.ExploitTime;
        }

        public override int GetHashCode()
        {
            return 397
                ^ ResourceType.GetHashCode()
                ^ Amount.GetHashCode()
                ^ AverageAcquisition.GetHashCode()
                ^ ExploitTime.GetHashCode();
        }

        public override string ToString()
        {
            return $"ResourcesSpot: {ResourceType} {Amount} {AverageAcquisition} {ExploitTime}";
        }
    }
}
