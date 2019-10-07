using System;
using Unity.Entities;
using Unity.AI.Planner;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using AI.Planner.Domains.Enums;

namespace AI.Planner.Domains
{
    [Serializable]
    public struct Need : ITrait, IEquatable<Need>
    {
        public const bool IsZeroSized = false;
        public AI.Planner.Domains.Enums.NeedType NeedType;
        public System.Single Urgency;
        public System.Single ChangePerSecond;

        public void SetField(string fieldName, object value)
        {
            switch (fieldName)
            {
                case nameof(NeedType):
                    NeedType = (AI.Planner.Domains.Enums.NeedType)Enum.ToObject(typeof(AI.Planner.Domains.Enums.NeedType), value);
                    break;
                case nameof(Urgency):
                    Urgency = (System.Single)value;
                    break;
                case nameof(ChangePerSecond):
                    ChangePerSecond = (System.Single)value;
                    break;
            }
        }

        public bool Equals(Need other)
        {
            return NeedType == other.NeedType && Urgency == other.Urgency && ChangePerSecond == other.ChangePerSecond;
        }

        public override int GetHashCode()
        {
            return 397
                ^ NeedType.GetHashCode()
                ^ Urgency.GetHashCode()
                ^ ChangePerSecond.GetHashCode();
        }

        public override string ToString()
        {
            return $"Need: {NeedType} {Urgency} {ChangePerSecond}";
        }
    }
}
