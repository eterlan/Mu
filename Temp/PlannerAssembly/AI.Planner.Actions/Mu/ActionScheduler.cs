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
            Exploit.ActionGuid,
            Navigate.ActionGuid,
            Pickup.ActionGuid,
            Sleep.ActionGuid,
        };

        static Dictionary<Guid, string> s_ActionGuidToNameLookup = new Dictionary<Guid,string>()
        {
            { Consume.ActionGuid, nameof(Consume) },
            { Exploit.ActionGuid, nameof(Exploit) },
            { Navigate.ActionGuid, nameof(Navigate) },
            { Pickup.ActionGuid, nameof(Pickup) },
            { Sleep.ActionGuid, nameof(Sleep) },
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
            var ExploitDataContext = StateManager.GetStateDataContext();
            var ExploitECB = new EntityCommandBuffer(Allocator.TempJob);
            ExploitDataContext.EntityCommandBuffer = ExploitECB.ToConcurrent();
            var NavigateDataContext = StateManager.GetStateDataContext();
            var NavigateECB = new EntityCommandBuffer(Allocator.TempJob);
            NavigateDataContext.EntityCommandBuffer = NavigateECB.ToConcurrent();
            var PickupDataContext = StateManager.GetStateDataContext();
            var PickupECB = new EntityCommandBuffer(Allocator.TempJob);
            PickupDataContext.EntityCommandBuffer = PickupECB.ToConcurrent();
            var SleepDataContext = StateManager.GetStateDataContext();
            var SleepECB = new EntityCommandBuffer(Allocator.TempJob);
            SleepDataContext.EntityCommandBuffer = SleepECB.ToConcurrent();

            var allActionJobs = new NativeArray<JobHandle>(5, Allocator.TempJob)
            {
                [0] = new Consume(UnexpandedStates, ConsumeDataContext).Schedule(UnexpandedStates, 0, inputDeps),
                [1] = new Exploit(UnexpandedStates, ExploitDataContext).Schedule(UnexpandedStates, 0, inputDeps),
                [2] = new Navigate(UnexpandedStates, NavigateDataContext).Schedule(UnexpandedStates, 0, inputDeps),
                [3] = new Pickup(UnexpandedStates, PickupDataContext).Schedule(UnexpandedStates, 0, inputDeps),
                [4] = new Sleep(UnexpandedStates, SleepDataContext).Schedule(UnexpandedStates, 0, inputDeps),
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

            ExploitECB.Playback(entityManager);
            for (int i = 0; i < UnexpandedStates.Length; i++)
            {
                var stateEntity = UnexpandedStates[i].Entity;
                var ExploitRefs = entityManager.GetBuffer<ExploitFixupReference>(stateEntity);
                for (int j = 0; j < ExploitRefs.Length; j++)
                    CreatedStateInfo.Enqueue(ExploitRefs[j].TransitionInfo);
                entityManager.RemoveComponent<ExploitFixupReference>(stateEntity);
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

            PickupECB.Playback(entityManager);
            for (int i = 0; i < UnexpandedStates.Length; i++)
            {
                var stateEntity = UnexpandedStates[i].Entity;
                var PickupRefs = entityManager.GetBuffer<PickupFixupReference>(stateEntity);
                for (int j = 0; j < PickupRefs.Length; j++)
                    CreatedStateInfo.Enqueue(PickupRefs[j].TransitionInfo);
                entityManager.RemoveComponent<PickupFixupReference>(stateEntity);
            }

            SleepECB.Playback(entityManager);
            for (int i = 0; i < UnexpandedStates.Length; i++)
            {
                var stateEntity = UnexpandedStates[i].Entity;
                var SleepRefs = entityManager.GetBuffer<SleepFixupReference>(stateEntity);
                for (int j = 0; j < SleepRefs.Length; j++)
                    CreatedStateInfo.Enqueue(SleepRefs[j].TransitionInfo);
                entityManager.RemoveComponent<SleepFixupReference>(stateEntity);
            }

            allActionJobs.Dispose();
            ConsumeECB.Dispose();
            ExploitECB.Dispose();
            NavigateECB.Dispose();
            PickupECB.Dispose();
            SleepECB.Dispose();

            return default;
        }
    }
}
