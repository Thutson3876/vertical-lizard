using UnityEngine;

public class RoarState : FiniteState
{
    private float _timeSinceRoar = 0f;
    private float _roarTime = 0.75f;
    public override void OnEnter(Creature creature, FiniteState previousState)
    {
        creature.PlayScreamAnimation();
        _timeSinceRoar = Time.unscaledTime + _roarTime;
    }

    public override void OnExit(Creature creature, FiniteState nextState)
    {
    }

    public override void OnTick(Creature creature)
    {
        if (Time.unscaledTime >= _timeSinceRoar)
        {
            creature.ChangeState(new FollowState());
        }
    }

    public override Vector3? GetTargetPosition(Creature creature)
    {
        return null;
    }

    public override void OnUpdate(Creature creature)
    {
    }
}