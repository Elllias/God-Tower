﻿using System;
using NTC.Global.Cache;
using UnityEngine;

public class ActiveAbilityGiver : MonoCache, IGiveAbility
{
    [SerializeField] private bool canGiveAbility;
    [SerializeField] private GameObject abilityPrefab;

    private ITrackingGiveAbilityOpportunity _tracker;
    private IHealthHandler _healthHandler;
    
    private void Awake()
    {
        if (abilityPrefab.GetComponent<IActiveAbility>() == null)
        {
            Debug.LogError("No ability in ability prefab on ability giver wtf");
        }

        _tracker = Get<ITrackingGiveAbilityOpportunity>();
        _healthHandler = Get<IHealthHandler>();
    }

    protected override void OnEnabled()
    {
        _tracker.OnCanGiveAbility += SetCanGive;
    }

    protected override void OnDisabled()
    {
        _tracker.OnCanGiveAbility -= SetCanGive;
    }

    public GameObject GetAbilityPrefab()
    {
        if (_healthHandler != null)
            _healthHandler.Die();
        return abilityPrefab;
    }

    public bool CanGiveAbility()
    {
        return canGiveAbility;
    }

    private void SetCanGive()
    {
        canGiveAbility = true;
    }
}