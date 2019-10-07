using System;
using Unity.AI.Planner;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.AI.Planner.DomainLanguage.TraitBased;

#if PLANNER_DOMAIN_GENERATED
using AI.Planner.Actions.Mu;
using AI.Planner.Domains;
#endif

#if PLANNER_DOMAIN_GENERATED
[Serializable]
public class Mu : BaseAgent<Mu, DomainObject, StateEntityKey, StateData, StateDataContext, ActionScheduler, 
NeedHeuristic, TerminationEvaluator, StateManager, ActionKey>
{
    public bool Dead { get; set; }
    public static ComponentType[] NeedFilter;
    public static ComponentType[] InventoryFilter;

    void Awake()
    {
        NeedFilter = new ComponentType[] { typeof(Need) };
        InventoryFilter = new ComponentType[] { typeof(Inventory) };
    }

    protected override void Update()
    {
        if (!Dead)
            base.Update();
    }
}

public struct NeedHeuristic : IHeuristic<StateData>
{
    public float Evaluate(StateData stateData)
    {
        var totalNeedsUrgency = 0f;

        // Resources
        var domainObjects = new NativeList<(DomainObject, int)>(4, Allocator.Temp);
        foreach (var (_, domainObjectIndex) in stateData.GetDomainObjects(domainObjects, Mu.NeedFilter))
        {
            var needTrait = stateData.GetTraitOnObjectAtIndex<Need>(domainObjectIndex);
            totalNeedsUrgency += needTrait.Urgency;
        }

        float value = 50;

        // Score based on total urgency over all needs (0 -> 300).
        if (totalNeedsUrgency > 50)
            value = 0;
        if (totalNeedsUrgency > 100)
            value = -30;
        if (totalNeedsUrgency > 150)
            value = -50;
        if (totalNeedsUrgency > 200)
            value = -150;

        domainObjects.Clear();
        foreach (var (_, domainObjectIndex) in stateData.GetDomainObjects(domainObjects, Mu.InventoryFilter))
        {
            var inventoryTrait = stateData.GetTraitOnObjectAtIndex<Inventory>(domainObjectIndex);
            value += inventoryTrait.Amount * 10;
        }
        domainObjects.Dispose();

        return value;
    }
}
#endif
