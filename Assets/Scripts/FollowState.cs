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
        }

        if (NavMesh.SamplePosition(creature.Target.position.ToXZ(), out var hit, 1f, 1))
        {
            Debug.Log("GO");
            _currentTarget = hit.position;
            creature.transform.forward = (_currentTarget - creature.transform.position).ToXZ().normalized;
        }
    }

    public override void OnUpdate(Creature creature)
    {
        creature.transform.position = Vector3.MoveTowards(creature.transform.position,
            _currentTarget.ToXZ() + new Vector3(0f, creature.transform.position.y, 0f),
            Time.deltaTime * creaturingSpeed);
    }
}