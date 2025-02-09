using System;
using PrimeTween;
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
        if (Vector3.Distance(creature.Target.position, creature.transform.position) < 2.5f)
        {
            Debug.Log("kill");
            creature.PlayAttackAnimation();
            Tween.Delay(creature.attackTime, () =>
            {
                if (Vector3.Distance(creature.Target.position, creature.transform.position) < 2.5f)
                {
                    AudioManager.PlaySound(creature.attackClip, creature.transform.position, 0.6f, 1f);
                    PlayerHealth.Instance.TakeDamage();
                }
            });
            creature.ChangeState(new IdleState(creature.idleTimeAfterAttacking));
        }
        
        _currentTarget = creature.Target.position;
    }

    public override Vector3? GetTargetPosition(Creature creature)
    {
        return _currentTarget;
    }

    public override void OnUpdate(Creature creature)
    {

    }
}