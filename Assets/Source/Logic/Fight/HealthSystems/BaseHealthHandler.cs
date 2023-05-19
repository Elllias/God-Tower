﻿using System;
using System.Linq;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;
using Zenject;

public class BaseHealthHandler : MonoCache, IHealthHandler, ITrackingGiveAbilityOpportunity
{
    [SerializeField] private bool canDie = true;
    [SerializeField] private bool canStartDying = true;
    [SerializeField] private bool oneHealthDeathProtection;
    [SerializeField] private float health;
    [SerializeField] private float damageImmuneTime = 0.1f;
    [SerializeField] private float dyingDuration = 5;

    private float _maxHealth;

    private float _timer;
    private float _dyingTimer;

    private bool _dying;

    private ITakeHit[] _hitTakers;
    private IWeakPoint[] _weakPoints;

    public event Action OnHit;
    public event Action<float> OnHealthChanged;
    public event Action OnDying;
    public event Action OnDied;
    public event Action OnCanGiveAbility;
    public event Action OnNotCanGiveAbility;
    public event Action OnRevive;

    private void Awake()
    {
        _maxHealth = health;
        
        _hitTakers = GetComponentsInChildren<ITakeHit>();
        _weakPoints = GetComponentsInChildren<IWeakPoint>();
    }

    protected override void OnEnabled()
    {
        for (var i = 0; i < _hitTakers.Length; i++)
        {
            _hitTakers[i].OnTakeHit += HandleHit;
        }
    }

    protected override void OnDisabled()
    {
        for (var i = 0; i < _hitTakers.Length; i++)
        {
            _hitTakers[i].OnTakeHit -= HandleHit;
        }
    }

    protected override void Run()
    {
        if (_timer > 0)
            _timer -= Time.deltaTime;

        if (_dyingTimer > 0 || _dying)
        {
            _dyingTimer -= Time.deltaTime;
            if (_dyingTimer <= 0)
            {
                Revive();
            }
        }
    }
    
    public void HandleHit(float damage)
    {
        if (_timer > 0)
            return;
        
        OnHit?.Invoke();
        _timer = damageImmuneTime;
        RemoveHealth(damage);
    }

    public void StartDying()
    {
        if (!canStartDying)
            return;
        
        _dying = true;
        _dyingTimer = dyingDuration;
        OnDying?.Invoke();
        OnCanGiveAbility?.Invoke();
    }

    public void Die(bool order = false)
    {
        if (!canDie && !order)
            return;
        
        OnDied?.Invoke();
        NightPool.Despawn(this);
    }

    public void Revive()
    {
        _dying = false;
        health = _maxHealth;
        OnNotCanGiveAbility?.Invoke();
        OnRevive?.Invoke();
    }

    public void AddHealth(float addValue)
    {
        SetHealth(health + addValue);
        health = Mathf.Clamp(health, 0, _maxHealth);
    }

    public void RemoveHealth(float removeValue)
    {
        SetHealth(health - removeValue);
        if (health <= 0)
        {
            StartDying();
        }

        if (health < -2000 && canDie)
        {
            Die();
        }
    }

    public void SetHealth(float value)
    {
        if (health > 1 && value <= 0)
            value = 1;
        health = value;
        OnHealthChanged?.Invoke(health);
    }

    public float GetHealth()
    {
        return health;
    }

    public bool IsDead() => _dying;

    public float GetReviveTime()
    {
        return dyingDuration;
    }

    public float GetCurrentReviveTimer()
    {
        return _dyingTimer;
    }
}
