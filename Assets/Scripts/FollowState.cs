using System;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class FollowState : FiniteState
{
    public float creaturingSpeed = 2.5f;
    private Vector3 _currentTarget;
    public override void OnEnter(Creature creature, FiniteState previousState)
    {
        if (creature.Target == null)
        {
            Debug.Log("no fuckin target");
        }
    }

    public override void OnExit(Creature creature, FiniteState nextState)
    {
    }

    public override void OnTick(Creature creature)
    {
        if (creature.Target == null)
        {
            creature.ChangeState(new IdleState());
            return;
        }
        if (Vector3.SqrMagnitude(creature.transform.position.ToXZ() - creature.Target.position.ToXZ()) < 0.1f * 0.1f)
        {
            Debug.Log("kill");
            creature.PlayAttackAnimation();
        }

        if (NavMesh.SamplePosition(creature.Target.position, out var hit, 1f, 1))
        {
            Debug.Log("GO");
            _currentTarget = hit.position;
        }
        else
        {
            _currentTarget = creature.Target.position;
        }
    }

    public override Vector3? GetTargetPosition(Creature creature)
    {
        return _currentTarget;
    }

    public override void OnUpdate(Creature creature)
    {

    }
}