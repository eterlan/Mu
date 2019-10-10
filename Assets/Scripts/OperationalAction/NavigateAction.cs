
using AI.Planner.Domains;
using Unity.AI.Planner.Agent;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using Unity.Collections;
using UnityEngine;

public class NavigateAction : MuAction
{
    //ActionKey
    private int m_actorLocKey = 0;
    private int m_targetLocKey = 1;
    private int m_time = 2;
    
    private Vector3 m_targetPos;
    private Vector3 m_targetOrientation;
    private float m_predictTime;
    private bool m_arrived;
    private float m_speed;
    private float m_maxSpeed = 2f;
    
    public override void BeginExecution
    (
        StateData stateData,
        ActionKey actionKey,
        Mu        actor
    )
    {
        base.BeginExecution(stateData, actionKey, actor);
        
        // Calculate PredictTime
        var domainObjects = new NativeList<(DomainObject, int)>(4, Allocator.TempJob);
        m_targetPos = stateData.GetTraitOnObjectAtIndex<Location>(actionKey[m_targetLocKey]).Position;
        var actorPos = stateData.GetTraitOnObjectAtIndex<Location>(actionKey[m_actorLocKey]).Position;
        m_targetOrientation = Vector3.Normalize(m_targetPos - actorPos);
        var distance = Vector3.Distance(actorPos, m_targetPos);
        m_predictTime = distance / m_maxSpeed;
    }

    public override void ContinueExecution
    (
        StateData stateData,
        ActionKey actionKey,
        Mu        actor
    )
    {
        base.ContinueExecution(stateData, actionKey, actor);
        
        // Lerp Speed
        var actorLocation = stateData.GetTraitOnObjectAtIndex<Location>(actionKey[m_actorLocKey]);
        m_speed = Mathf.Lerp(m_speed, m_maxSpeed, 0.1f);
        // Rotate to Target
        actorLocation.Forward = Vector3.Lerp(actorLocation.Forward, m_targetOrientation, 0.1f);
        // Move forward
        actorLocation.Position += m_speed * UnityEngine.Time.deltaTime * actorLocation.Forward;
        stateData.SetTraitOnObjectAtIndex(actorLocation, actionKey[m_actorLocKey]);
        
        // Check Arrived.
        m_arrived = Vector3.Distance(actorLocation.Position, m_targetPos) < 0.1f;
    }

    public override void EndExecution
    (
        StateData stateData,
        ActionKey actionKey,
        Mu        actor
    )
    {
        base.EndExecution(stateData, actionKey, actor);
        
        //Todo: Set Mu Location. What if I don't do it?
        var targetLocation = stateData.GetTraitOnObjectAtIndex<Location>(m_targetLocKey);
        var agentLocation = stateData.GetTraitOnObjectAtIndex<Location>(m_actorLocKey);
        agentLocation.Position = targetLocation.Position;
        stateData.SetTraitOnObjectAtIndex(agentLocation, m_targetLocKey);
    }
    
    // Set EndExecute Condition.
    public override OperationalActionStatus Status
    (StateData state,
     ActionKey action,
     Mu        agent)
    {
        return m_arrived && UnityEngine.Time.time - StartTime > m_predictTime ? OperationalActionStatus.Completed : OperationalActionStatus.InProgress;
    }
}
