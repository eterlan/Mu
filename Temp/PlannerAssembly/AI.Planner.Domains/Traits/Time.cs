using System;
using Unity.Entities;
using Unity.AI.Planner;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using AI.Planner.Domains.Enums;

namespace AI.Planner.Domains
{
    [Serializable]
    public struct Time : ITrait, IEquatable<Time>
    {
        public const bool IsZeroSized = false;
        public System.Single Value;

        public void SetField(string fieldName, object value)
        {
            switch (fieldName)
            {
                case nameof(Value):
                    Value = (System.Single)value;
                    break;
            }
        }

        public bool Equals(Time other)
        {
            return Value == other.Value;
        }

        public override int GetHashCode()
        {
            return 397
                ^ Value.GetHashCode();
        }

        public override string ToString()
        {
            return $"Time: {Value}";
        }
    }
}
