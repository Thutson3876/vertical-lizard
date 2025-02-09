using System;
using UnityEngine;

[Serializable]
public abstract class FiniteState
{
    public abstract void OnEnter(Creature creature, FiniteState previousState);
    public abstract void OnExit(Creature creature, FiniteState nextState);
    public abstract void OnTick(Creature creature);
    
    public abstract Vector3? GetTargetPosition(Creature creature);
    public abstract void OnUpdate(Creature creature);
}