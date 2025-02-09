using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[Serializable]
public class WanderState : FiniteState
{
    public float speed = 3f;
    public float scanRadius = 3f;
    private Vector3 _currentTarget;

    public override void OnEnter(Creature creature, FiniteState previousState)
    {
        GetRandomTarget(creature.transform);
        if (float.IsInfinity(_currentTarget.x) || float.IsNaN(_currentTarget.y) || float.IsNaN(_currentTarget.z))
        {
            GetRandomTarget(creature.transform);
        }
    }

    public override void OnExit(Creature creature, FiniteState nextState)
    {
    }

    public override void OnTick(Creature creature)
    {
        if (creature.Target != null)
        {
            creature.ChangeState(new FollowState());
        }
        if (Vector3.SqrMagnitude(creature.transform.position.ToXZ() - _currentTarget.ToXZ()) < 0.1f * 0.1f)
        {
            GetRandomTarget(creature.transform);
        }
    }

    public override Vector3? GetTargetPosition(Creature creature)
    {
        return _currentTarget;
    }

    public override void OnUpdate(Creature creature)
    {
        
    }

    private void GetRandomTarget(Transform tr)
    {
        Vector3 randomDir = Random.insideUnitSphere * scanRadius;
        randomDir += tr.position;
        if (NavMesh.SamplePosition(randomDir, out var hit, scanRadius, 1))
        {
            _currentTarget = hit.position;
        }
        else
        {
            Debug.Log("fuck!");
        }
    }
}