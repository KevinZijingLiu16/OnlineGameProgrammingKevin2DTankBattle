using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class Health : NetworkBehaviour
{
    [field: SerializeField] public int MaxHealth { get; private set; } = 100; // The maximum health of the player,no need to sync this value

    public NetworkVariable<int> CurrentHealth = new NetworkVariable<int>();//need to sync this value

    private bool isDead;

    public Action<Health> OnDie;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        CurrentHealth.Value = MaxHealth;
    }

    public void TakeDamage(int damageValue)
    {
        ModifyHealth(-damageValue);
    }

    public void RestoreHealth(int healValue)
    {
        ModifyHealth(healValue);
    }

    private void ModifyHealth(int value)
    {
        if (isDead) { return; }

        int newHealth = CurrentHealth.Value + value;
        CurrentHealth.Value = Mathf.Clamp(newHealth, 0, MaxHealth);

        if (CurrentHealth.Value == 0)
        {
            OnDie?.Invoke(this);
            isDead = true;
        }
    }


}
