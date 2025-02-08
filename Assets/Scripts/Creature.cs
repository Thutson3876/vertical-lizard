using System;
using System.Collections;
using System.Collections.Generic;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.AI;

public class Creature : MonoBehaviour
{
    [SerializeField] [SerializeReference] private List<FiniteState> states = new List<FiniteState>();
    [SerializeField] private int tickRate = 30;
    [SerializeField] private Transform targetTr;
    public Transform Target { get; set; }
    private FiniteState _currentState;
    private float _timeSinceLastTick = 0f;
    private float _tickTime;

    [Button]
    private void AddWanderState()
    {
        states.Add(new WanderState());
    }

    [Button]
    private void AddFollowState() => states.Add(new FollowState());

    [Button]
    private void AddIdleState() => states.Add(new IdleState());


    private void Awake()
    {
        _currentState = states[0];
        _tickTime = 1f / tickRate;
        _timeSinceLastTick = Time.unscaledTime + _tickTime;
        _currentState.OnEnter(this, null);
    }

    private void Update()
    {
        _currentState.OnUpdate(this);
        if (Time.unscaledTime <= _timeSinceLastTick)
        {
            return;
        }

        LookForTarget();
        _currentState.OnTick(this);
        _timeSinceLastTick = Time.unscaledTime + _tickTime;
    }

    public void ChangeState(FiniteState state)
    {
        var prevState = _currentState;
        _currentState.OnExit(this, state);
        _currentState = state;
        _currentState.OnEnter(this, prevState);
    }

    private void LookForTarget()
    {
        var fwd = transform.forward;
        var toTarget = (targetTr.position - transform.position);
        var mag = toTarget.magnitude;

        if (mag < 3.0f && Vector3.Dot(fwd, toTarget.normalized) > 0.7f)
        {
            Target = targetTr;
        }
        else
        {
            Target = null;
        }
    }
}