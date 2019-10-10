using System;
using Unity.Entities;
using Unity.AI.Planner;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using AI.Planner.Domains.Enums;

namespace AI.Planner.Domains
{
    [Serializable]
    public struct Bed : ITrait, IEquatable<Bed>
    {
        public const bool IsZeroSized = false;
        public System.Int64 Restoration;
        public System.Int64 SleepDuration;

        public void SetField(string fieldName, object value)
        {
            switch (fieldName)
            {
                case nameof(Restoration):
                    Restoration = (System.Int64)value;
                    break;
                case nameof(SleepDuration):
                    SleepDuration = (System.Int64)value;
                    break;
            }
        }

        public bool Equals(Bed other)
        {
            return Restoration == other.Restoration && SleepDuration == other.SleepDuration;
        }

        public override int GetHashCode()
        {
            return 397
                ^ Restoration.GetHashCode()
                ^ SleepDuration.GetHashCode();
        }

        public override string ToString()
        {
            return $"Bed: {Restoration} {SleepDuration}";
        }
    }
}
