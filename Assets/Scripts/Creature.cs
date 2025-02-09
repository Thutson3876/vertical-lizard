using System;
using System.Collections;
using System.Collections.Generic;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.AI;

public class Creature : MonoBehaviour
{
    private static readonly int WalkBlend = Animator.StringToHash("WalkBlend");
    private static readonly int Death = Animator.StringToHash("Death");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Scream = Animator.StringToHash("Scream");
    [SerializeField] private int tickRate = 30;
    [SerializeField] private Transform targetTr;
    [SerializeField] private float creatureSpeed = 3f;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float targetLostTime = 5f;
    [SerializeField] private float minTargetDistance = 3f;
    [SerializeField] private float fov = 110f;
    public Transform Target { get; set; }
    private FiniteState _currentState;
    private float _timeSinceLastTick = 0f;
    private float _tickTime;
    private float _timeSinceTarget = 0f;
    private Rigidbody _rigidbody;
    private Animator _animator;

    private void Awake()
    {
        _currentState = new IdleState();
        _tickTime = 1f / tickRate;
        _timeSinceLastTick = Time.unscaledTime + _tickTime;
        _timeSinceTarget = Time.unscaledTime + targetLostTime;
        _currentState.OnEnter(this, null);
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
        if (targetTr == null)
        {
            targetTr = Camera.main.transform;
        }
    }

    private void Update()
    {
        _currentState.OnUpdate(this);
        if (Time.unscaledTime <= _timeSinceLastTick)
        {
            return;
        }

        LookForTarget();
        _animator.SetFloat(WalkBlend, Mathf.InverseLerp(0f, agent.speed, agent.velocity.magnitude));
        _currentState.OnTick(this);
        _timeSinceLastTick = Time.unscaledTime + _tickTime;
    }

    public void PlayDeathAnimation()
    {
        _animator.SetTrigger(Death);
    }

    public void PlayAttackAnimation()
    {
        _animator.SetTrigger(Attack);
    }

    public void PlayScreamAnimation()
    {
        _animator.SetTrigger(Scream);
    }

    private void FixedUpdate()
    {
        var target = _currentState.GetTargetPosition(this);
        if (target != null)
        {
            transform.LookAt(target.Value, Vector3.up);
            agent.SetDestination(target.Value);
        }
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
        bool targetFound = mag <= minTargetDistance && Vector3.Angle(toTarget, fwd) < fov * 0.5f;
        if (targetFound)
        {
            Target = targetTr;
            _timeSinceTarget = Time.unscaledTime + targetLostTime;
        }
        else
        {
            if (Target != null && Time.unscaledTime >= _timeSinceTarget)
            {
                Debug.Log("target lost");
                Target = null;
            }
            //do something with timer to go back to no target            
        }
    }
}