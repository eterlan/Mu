using AI.Planner.Domains;
using Unity.AI.Planner.Agent;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using Unity.Collections;
using Time = UnityEngine.Time;

public abstract class MuAction : IOperationalAction<Mu, StateData, ActionKey>
{
    // AnimationNotImplemented -> True for now.
    public bool AnimationComplete = true;
    protected int AccumulateTime;
    protected float MinUnitTime = 1f;
    protected float StartTime;
    
    public virtual void  BeginExecution
    (StateData state,
     ActionKey action,
     Mu        agent)
    {
        StartTime = Time.time;
        AccumulateTime = 0;
        //AnimationComplete = false;
    }

    private void UpdateNeed(StateData stateData, Mu actor)
    {
        // 1 time per second.
        if (Time.time - StartTime > AccumulateTime)
        {
            AccumulateTime++;

            var domainObjects = new NativeList<(DomainObject, int)>(4, Allocator.TempJob);
            
            // Update Time Trait in domainObjects which contains Time.
            foreach (var (domainObject, domainObjectIndex) in 
                stateData.GetDomainObjects(domainObjects, typeof(AI.Planner.Domains.Time)))
            {
                var time = stateData.GetTraitOnObjectAtIndex<AI.Planner.Domains.Time>(domainObjectIndex);
                time.Value++;
                stateData.SetTraitOnObjectAtIndex(time, domainObjectIndex);
            }
            
            domainObjects.Clear();
            
            // Update Need Trait in domainObjects which contains Need.
            foreach (var (domainObject, domainObjectIndex) in
                stateData.GetDomainObjects(domainObjects, typeof(Need)))
            {
                var need = stateData.GetTraitOnObjectAtIndex<Need>(domainObjectIndex);
                need.Urgency += need.ChangePerSecond;
                stateData.SetTraitOnObjectAtIndex(need, domainObjectIndex);
                
                // Check for Death
                if (need.Urgency > 100)
                {
                    actor.Dead = true; 
                    //m_Animator.SetBool(m_DeathAnimationFlags[need.NeedType], true);
                }
            }

            domainObjects.Dispose();
        }
    }

    public virtual void ContinueExecution
    (StateData state,
     ActionKey action,
     Mu        agent)
    {
        UpdateNeed(state,agent);
    }

    public virtual void EndExecution
    (StateData state,
     ActionKey action,
     Mu        agent)
    {
        UpdateNeed(state,agent);
    }

    public virtual OperationalActionStatus Status
    (StateData state,
     ActionKey action,
     Mu        agent)
    {
        return AnimationComplete && Time.time - StartTime > MinUnitTime
            ? OperationalActionStatus.Completed
            : OperationalActionStatus.InProgress;
    }
}
