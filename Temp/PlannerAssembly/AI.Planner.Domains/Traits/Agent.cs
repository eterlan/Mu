using System;
using Unity.Entities;
using Unity.AI.Planner;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using AI.Planner.Domains.Enums;

namespace AI.Planner.Domains
{
    [Serializable]
    public struct Agent : ITrait, IEquatable<Agent>
    {
        public const bool IsZeroSized = true;

        public void SetField(string fieldName, object value)
        {
        }

        public bool Equals(Agent other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            return ComponentType.ReadOnly<Agent>().TypeIndex;
        }

        public override string ToString()
        {
            return $"Agent";
        }
    }
}
