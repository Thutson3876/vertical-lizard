using System;
using UnityEngine;

[Serializable]
public class IdleState : FiniteState
{
    private float _idleDuration = 5f;
    private float _idleTime = 0f;
    public override void OnEnter(Creature creature, FiniteState previousState)
    {
        _idleTime = Time.unscaledTime + _idleDuration;
    }

    public override void OnExit(Creature creature, FiniteState nextState)
    {
    }

    public override void OnTick(Creature creature)
    {
        if (Time.unscaledTime < _idleTime)
        {
            return;
        }
        if (creature.Target != null)
        {
            creature.ChangeState(new FollowState());
        }
        else
        {
            creature.ChangeState(new WanderState());
        }
        _idleTime = Time.unscaledTime + _idleDuration;
    }

    public override Vector3? GetTargetPosition(Creature creature)
    {
        return null;
    }

    public override void OnUpdate(Creature creature)
    {
        
    }
}