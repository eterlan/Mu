using System;
using Unity.Entities;
using Unity.AI.Planner;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using AI.Planner.Domains.Enums;

namespace AI.Planner.Domains
{
    [Serializable]
    public struct Infrastructure : ITrait, IEquatable<Infrastructure>
    {
        public const bool IsZeroSized = true;

        public void SetField(string fieldName, object value)
        {
        }

        public bool Equals(Infrastructure other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            return ComponentType.ReadOnly<Infrastructure>().TypeIndex;
        }

        public override string ToString()
        {
            return $"Infrastructure";
        }
    }
}
