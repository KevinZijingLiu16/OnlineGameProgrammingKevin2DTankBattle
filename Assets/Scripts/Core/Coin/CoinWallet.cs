using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

    public void SpendCoins(int costToFire)
    {
        TotalCoins.Value -= costToFire;
    }

    private void OnTriggerEnter2D(Collider2D colision)
    {
        if (!colision.TryGetComponent<Coin>(out Coin coin)) { return; } // If the object doesn't have a Coin component, return

        int coinValue = coin.Collect();

        Debug.Log($"Collected {coinValue} coins");

        if (!IsServer) { return; }

        TotalCoins.Value += coinValue;
    }
}
