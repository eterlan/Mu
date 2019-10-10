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
    struct Navigate : IJobParallelForDefer
    {
        public static Guid ActionGuid = Guid.NewGuid();
        
        const int k_AgentIndex = 0;
        const int k_ToIndex = 1;
        const int k_TimeIndex = 2;
        const int k_MaxArguments = 3;
        static readonly ComponentType[] s_AgentFilter = {ComponentType.ReadWrite<Location>(),ComponentType.ReadWrite<Agent>(),};
        static readonly ComponentType[] s_ToFilter = {ComponentType.ReadWrite<Location>(),};
        static readonly ComponentType[] s_TimeFilter = {ComponentType.ReadWrite<Time>(),};

        [ReadOnly] NativeArray<StateEntityKey> m_StatesToExpand;
        [ReadOnly] StateDataContext m_StateDataContext;

        internal Navigate(NativeList<StateEntityKey> statesToExpand, StateDataContext stateDataContext)
        {
            m_StatesToExpand = statesToExpand.AsDeferredJobArray();
            m_StateDataContext = stateDataContext;
        }

        void GenerateArgumentPermutations(StateData stateData, NativeList<ActionKey> argumentPermutations)
        {
            var AgentObjects = new NativeList<(DomainObject, int)>(2, Allocator.Temp);
            stateData.GetDomainObjects(AgentObjects, s_AgentFilter);
            var ToObjects = new NativeList<(DomainObject, int)>(2, Allocator.Temp);
            stateData.GetDomainObjects(ToObjects, s_ToFilter);
            var TimeObjects = new NativeList<(DomainObject, int)>(2, Allocator.Temp);
            stateData.GetDomainObjects(TimeObjects, s_TimeFilter);
            var LocationBuffer = stateData.LocationBuffer;
            
            for (int i0 = 0; i0 < AgentObjects.Length; i0++)
            {
                var (AgentObject, AgentIndex) = AgentObjects[i0];
                
            
            for (int i1 = 0; i1 < ToObjects.Length; i1++)
            {
                var (ToObject, ToIndex) = ToObjects[i1];
                
                if (!(LocationBuffer[AgentObject.LocationIndex].Position != LocationBuffer[ToObject.LocationIndex].Position))
                    continue;
            
            for (int i2 = 0; i2 < TimeObjects.Length; i2++)
            {
                var (TimeObject, TimeIndex) = TimeObjects[i2];
                

                var actionKey = new ActionKey(k_MaxArguments) {
                                                        ActionGuid = ActionGuid,
                                                       [k_AgentIndex] = AgentIndex,
                                                       [k_ToIndex] = ToIndex,
                                                       [k_TimeIndex] = TimeIndex,
                                                    };


                argumentPermutations.Add(actionKey);
            }
            }
            }
                AgentObjects.Dispose();
                ToObjects.Dispose();
                TimeObjects.Dispose();
        }

        (StateEntityKey, ActionKey, ActionResult, StateEntityKey) ApplyEffects(ActionKey action, StateEntityKey originalStateEntityKey)
        {
            var originalState = m_StateDataContext.GetStateData(originalStateEntityKey);
            var originalStateObjectBuffer = originalState.DomainObjects;
            var originalAgentObject = originalStateObjectBuffer[action[k_AgentIndex]];
            var originalToObject = originalStateObjectBuffer[action[k_ToIndex]];

            var newState = m_StateDataContext.CopyStateData(originalState);
            var newLocationBuffer = newState.LocationBuffer;
            {
                var @Location = newLocationBuffer[originalAgentObject.LocationIndex];
                @Location.Position = newLocationBuffer[originalToObject.LocationIndex].Position;
                newLocationBuffer[originalAgentObject.LocationIndex] = @Location;
            }

            


            var reward = Reward(originalState, action, newState);
            var actionResult = new ActionResult { Probability = 1f, TransitionUtilityValue = reward };
            var resultingStateKey = m_StateDataContext.GetStateDataKey(newState);

            return (originalStateEntityKey, action, actionResult, resultingStateKey);
        }

        float Reward(StateData originalState, ActionKey action, StateData newState)
        {
            var reward = -2f;
            return reward;
        }

        public void Execute(int jobIndex)
        {
            m_StateDataContext.JobIndex = jobIndex; //todo check that all actions set the job index

            var stateEntityKey = m_StatesToExpand[jobIndex];
            var stateData = m_StateDataContext.GetStateData(stateEntityKey);

            var argumentPermutations = new NativeList<ActionKey>(4, Allocator.Temp);
            GenerateArgumentPermutations(stateData, argumentPermutations);

            var transitionInfo = new NativeArray<NavigateFixupReference>(argumentPermutations.Length, Allocator.Temp);
            for (var i = 0; i < argumentPermutations.Length; i++)
            {
                transitionInfo[i] = new NavigateFixupReference { TransitionInfo = ApplyEffects(argumentPermutations[i], stateEntityKey) };
            }

            // fixups
            var stateEntity = stateEntityKey.Entity;
            var fixupBuffer = m_StateDataContext.EntityCommandBuffer.AddBuffer<NavigateFixupReference>(jobIndex, stateEntity);
            fixupBuffer.CopyFrom(transitionInfo);

            transitionInfo.Dispose();
            argumentPermutations.Dispose();
        }

        
        public static T GetAgentTrait<T>(StateData state, ActionKey action) where T : struct, ITrait
        {
            return state.GetTraitOnObjectAtIndex<T>(action[k_AgentIndex]);
        }
        
        public static T GetToTrait<T>(StateData state, ActionKey action) where T : struct, ITrait
        {
            return state.GetTraitOnObjectAtIndex<T>(action[k_ToIndex]);
        }
        
        public static T GetTimeTrait<T>(StateData state, ActionKey action) where T : struct, ITrait
        {
            return state.GetTraitOnObjectAtIndex<T>(action[k_TimeIndex]);
        }
        
    }

    public struct NavigateFixupReference : IBufferElementData
    {
        public (StateEntityKey, ActionKey, ActionResult, StateEntityKey) TransitionInfo;
    }
}


