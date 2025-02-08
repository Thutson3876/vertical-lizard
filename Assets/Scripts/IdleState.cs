using System;
using UnityEngine;

[Serializable]
public class IdleState : FiniteState
{
    public override void OnEnter(Creature creature, FiniteState previousState)
    {
    }

    public override void OnExit(Creature creature, FiniteState nextState)
    {
    }

    public override void OnTick(Creature creature)
    {
    }
}