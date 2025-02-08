using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Creature : MonoBehaviour
{
    [SerializeField] [SerializeReference] private List<FiniteState> states = new List<FiniteState>();
    [SerializeField] private int tickRate = 30;
    [field: SerializeField]
    public NavMeshAgent NavMeshAgent { get; private set; }
    private FiniteState _currentState;
    private float _timeSinceLastTick = 0f;
    private float _tickTime;
    
    [ContextMenu("Add WanderState")]
    private void AddWanderState() => states.Add(new WanderState());

    private void Awake()
    {
        _currentState = states[0];
        _tickTime = 1f / tickRate;
        _timeSinceLastTick = Time.unscaledTime + _tickTime;
        _currentState.OnEnter(this, null);
    }

    private void Update()
    {
        if (Time.unscaledTime <= _timeSinceLastTick)
        {
            return;
        }

        _currentState.OnTick(this);
        _timeSinceLastTick = Time.unscaledTime + _tickTime;
    }

    private void ChangeState(FiniteState state)
    {
        var prevState = _currentState;
        _currentState.OnExit(this, state);
        _currentState = state;
        _currentState.OnEnter(this, prevState);
    }
}