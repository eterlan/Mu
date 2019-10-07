using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.AI.Planner;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using AI.Planner.Domains;
using AI.Planner.Domains.Enums;

namespace AI.Planner.Actions.Mu
{
    struct Consume : IJobParallelForDefer
    {
        public static Guid ActionGuid = Guid.NewGuid();
        
        const int k_InventoryIndex = 0;
        const int k_NeedIndex = 1;
        const int k_MaxArguments = 2;
        static readonly ComponentType[] s_InventoryFilter = {ComponentType.ReadWrite<Inventory>(),};
        static readonly ComponentType[] s_NeedFilter = {ComponentType.ReadWrite<Need>(),};

        [ReadOnly] NativeArray<StateEntityKey> m_StatesToExpand;
        [ReadOnly] StateDataContext m_StateDataContext;

        internal Consume(NativeList<StateEntityKey> statesToExpand, StateDataContext stateDataContext)
        {
            m_StatesToExpand = statesToExpand.AsDeferredJobArray();
            m_StateDataContext = stateDataContext;
        }

        void GenerateArgumentPermutations(StateData stateData, NativeList<ActionKey> argumentPermutations)
        {
            var InventoryObjects = new NativeList<(DomainObject, int)>(2, Allocator.Temp);
            stateData.GetDomainObjects(InventoryObjects, s_InventoryFilter);
            var NeedObjects = new NativeList<(DomainObject, int)>(2, Allocator.Temp);
            stateData.GetDomainObjects(NeedObjects, s_NeedFilter);
            var InventoryBuffer = stateData.InventoryBuffer;
            var NeedBuffer = stateData.NeedBuffer;
            
            for (int i0 = 0; i0 < InventoryObjects.Length; i0++)
            {
                var (InventoryObject, InventoryIndex) = InventoryObjects[i0];
                
                if (!(InventoryBuffer[InventoryObject.InventoryIndex].Amount > 0))
                    continue;
                
                
            
            for (int i1 = 0; i1 < NeedObjects.Length; i1++)
            {
                var (NeedObject, NeedIndex) = NeedObjects[i1];
                
                
                if (!(InventoryBuffer[InventoryObject.InventoryIndex].SatisfiesNeed == NeedBuffer[NeedObject.NeedIndex].NeedType))
                    continue;
                
                if (!(NeedBuffer[NeedObject.NeedIndex].Urgency > InventoryBuffer[InventoryObject.InventoryIndex].Restoration))
                    continue;

                var actionKey = new ActionKey(k_MaxArguments) {
                                                        ActionGuid = ActionGuid,
                                                       [k_InventoryIndex] = InventoryIndex,
                                                       [k_NeedIndex] = NeedIndex,
                                                    };


                argumentPermutations.Add(actionKey);
            }
            }
                InventoryObjects.Dispose();
                NeedObjects.Dispose();
        }

        (StateEntityKey, ActionKey, ActionResult, StateEntityKey) ApplyEffects(ActionKey action, StateEntityKey originalStateEntityKey)
        {
            var originalState = m_StateDataContext.GetStateData(originalStateEntityKey);
            var originalStateObjectBuffer = originalState.DomainObjects;

            var newState = m_StateDataContext.CopyStateData(originalState);

            


            var reward = Reward(originalState, action, newState);
            var actionResult = new ActionResult { Probability = 1f, TransitionUtilityValue = reward };
            var resultingStateKey = m_StateDataContext.GetStateDataKey(newState);

            return (originalStateEntityKey, action, actionResult, resultingStateKey);
        }

        float Reward(StateData originalState, ActionKey action, StateData newState)
        {
            var reward = 0f;
            return reward;
        }

        public void Execute(int jobIndex)
        {
            m_StateDataContext.JobIndex = jobIndex; //todo check that all actions set the job index

            var stateEntityKey = m_StatesToExpand[jobIndex];
            var stateData = m_StateDataContext.GetStateData(stateEntityKey);

            var argumentPermutations = new NativeList<ActionKey>(4, Allocator.Temp);
            GenerateArgumentPermutations(stateData, argumentPermutations);

            var transitionInfo = new NativeArray<ConsumeFixupReference>(argumentPermutations.Length, Allocator.Temp);
            for (var i = 0; i < argumentPermutations.Length; i++)
            {
                transitionInfo[i] = new ConsumeFixupReference { TransitionInfo = ApplyEffects(argumentPermutations[i], stateEntityKey) };
            }

            // fixups
            var stateEntity = stateEntityKey.Entity;
            var fixupBuffer = m_StateDataContext.EntityCommandBuffer.AddBuffer<ConsumeFixupReference>(jobIndex, stateEntity);
            fixupBuffer.CopyFrom(transitionInfo);

            transitionInfo.Dispose();
            argumentPermutations.Dispose();
        }

        
        public static T GetInventoryTrait<T>(StateData state, ActionKey action) where T : struct, ITrait
        {
            return state.GetTraitOnObjectAtIndex<T>(action[k_InventoryIndex]);
        }
        
        public static T GetNeedTrait<T>(StateData state, ActionKey action) where T : struct, ITrait
        {
            return state.GetTraitOnObjectAtIndex<T>(action[k_NeedIndex]);
        }
        
    }

    public struct ConsumeFixupReference : IBufferElementData
    {
        public (StateEntityKey, ActionKey, ActionResult, StateEntityKey) TransitionInfo;
    }
}


