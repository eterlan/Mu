using System;
using Unity.Entities;
using Unity.AI.Planner;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using AI.Planner.Domains.Enums;

namespace AI.Planner.Domains
{
    [Serializable]
    public struct ConsumableDispenser : ITrait, IEquatable<ConsumableDispenser>
    {
        public const bool IsZeroSized = false;
        public AI.Planner.Domains.Enums.ConsumableType ConsumableType;

        public void SetField(string fieldName, object value)
        {
            switch (fieldName)
            {
                case nameof(ConsumableType):
                    ConsumableType = (AI.Planner.Domains.Enums.ConsumableType)Enum.ToObject(typeof(AI.Planner.Domains.Enums.ConsumableType), value);
                    break;
            }
        }

        public bool Equals(ConsumableDispenser other)
        {
            return ConsumableType == other.ConsumableType;
        }

        public override int GetHashCode()
        {
            return 397
                ^ ConsumableType.GetHashCode();
        }

        public override string ToString()
        {
            return $"ConsumableDispenser: {ConsumableType}";
        }
    }
}
