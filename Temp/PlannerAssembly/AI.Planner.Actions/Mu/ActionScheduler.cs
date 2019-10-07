using System;
using System.Collections.Generic;
using AI.Planner.Domains;
using Unity.AI.Planner;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace AI.Planner.Actions.Mu
{
    using StateTransitionInfo = ValueTuple<StateEntityKey, ActionKey, ActionResult, StateEntityKey>;

    public class ActionScheduler :
        ITraitBasedActionScheduler<DomainObject, StateEntityKey, StateData, StateDataContext, StateManager, ActionKey, ActionResult>,
        IGetActionName
    {
        // Input
        public NativeList<StateEntityKey> UnexpandedStates { get; set; }
        public StateManager StateManager { get; set; }

        // Output
        public NativeQueue<StateTransitionInfo> CreatedStateInfo { get; set; }

        public Guid[] ActionGuids => s_ActionGuids;

        static Guid[] s_ActionGuids = {
            Consume.ActionGuid,
            Navigate.ActionGuid,
        };

        static Dictionary<Guid, string> s_ActionGuidToNameLookup = new Dictionary<Guid,string>()
        {
            { Consume.ActionGuid, nameof(Consume) },
            { Navigate.ActionGuid, nameof(Navigate) },
        };

        public string GetActionName(IActionKey actionKey)
        {
            s_ActionGuidToNameLookup.TryGetValue(((IActionKeyWithGuid)actionKey).ActionGuid, out var name);
            return name;
        }

        public JobHandle Schedule(JobHandle inputDeps)
        {
            var ConsumeDataContext = StateManager.GetStateDataContext();
            var ConsumeECB = new EntityCommandBuffer(Allocator.TempJob);
            ConsumeDataContext.EntityCommandBuffer = ConsumeECB.ToConcurrent();
            var NavigateDataContext = StateManager.GetStateDataContext();
            var NavigateECB = new EntityCommandBuffer(Allocator.TempJob);
            NavigateDataContext.EntityCommandBuffer = NavigateECB.ToConcurrent();

            var allActionJobs = new NativeArray<JobHandle>(2, Allocator.TempJob)
            {
                [0] = new Consume(UnexpandedStates, ConsumeDataContext).Schedule(UnexpandedStates, 0, inputDeps),
                [1] = new Navigate(UnexpandedStates, NavigateDataContext).Schedule(UnexpandedStates, 0, inputDeps),
            };

            JobHandle.CompleteAll(allActionJobs);

            // Playback entity changes and output state transition info
            var entityManager = StateManager.EntityManager;

            ConsumeECB.Playback(entityManager);
            for (int i = 0; i < UnexpandedStates.Length; i++)
            {
                var stateEntity = UnexpandedStates[i].Entity;
                var ConsumeRefs = entityManager.GetBuffer<ConsumeFixupReference>(stateEntity);
                for (int j = 0; j < ConsumeRefs.Length; j++)
                    CreatedStateInfo.Enqueue(ConsumeRefs[j].TransitionInfo);
                entityManager.RemoveComponent<ConsumeFixupReference>(stateEntity);
            }

            NavigateECB.Playback(entityManager);
            for (int i = 0; i < UnexpandedStates.Length; i++)
            {
                var stateEntity = UnexpandedStates[i].Entity;
                var NavigateRefs = entityManager.GetBuffer<NavigateFixupReference>(stateEntity);
                for (int j = 0; j < NavigateRefs.Length; j++)
                    CreatedStateInfo.Enqueue(NavigateRefs[j].TransitionInfo);
                entityManager.RemoveComponent<NavigateFixupReference>(stateEntity);
            }

            allActionJobs.Dispose();
            ConsumeECB.Dispose();
            NavigateECB.Dispose();

            return default;
        }
    }
}
