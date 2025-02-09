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
    [SerializeField] private float targetLostTime = 5f;
    [SerializeField] private float minTargetDistance = 3f;
    [SerializeField] private float fov = 110f;
    [SerializeField] private float initialIdleTime = 5f;
    [SerializeField] private float creatureSpeed = 3.5f;
    public float idleTimeAfterAttacking = 10f;
    [ShowInInspector]
    private string _currentStateName => _currentState != null ? _currentState.GetType().Name : "null";
    public Transform Target { get; set; }
    private FiniteState _currentState;
    private float _timeSinceLastTick = 0f;
    private float _tickTime;
    private float _timeSinceTarget = 0f;
    private Rigidbody _rigidbody;
    private Animator _animator;

    private void Awake()
    {
        _tickTime = 1f / tickRate;
        _timeSinceLastTick = Time.unscaledTime + _tickTime;
        _timeSinceTarget = Time.unscaledTime + targetLostTime;
        _currentState = new IdleState(200000);
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

        _animator.SetFloat(WalkBlend, Mathf.InverseLerp(0f, creatureSpeed, _rigidbody.velocity.magnitude));
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
            var lookPos = target.Value - transform.position;
            lookPos.y = 0f;
            transform.rotation = Quaternion.LookRotation(lookPos.normalized, Vector3.up);
            var targetVel = (target.Value - transform.position).normalized * creatureSpeed;
            targetVel.y = _rigidbody.velocity.y;
            _rigidbody.velocity = targetVel;
        }
    }

    public void ChangeState(FiniteState state)
    {
        var prevState = _currentState;
        _currentState.OnExit(this, state);
        _currentState = state;
        _currentState.OnEnter(this, prevState);
    }
    
    public void SetSeePlayer(bool seePlayer)
    {
        Target = seePlayer ? targetTr : null;
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