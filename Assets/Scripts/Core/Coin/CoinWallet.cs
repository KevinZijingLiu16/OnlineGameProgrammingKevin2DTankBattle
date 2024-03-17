using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    [Header("References")]

    [SerializeField] private Health health;
    [SerializeField] private BountyCoin BountyCoinPrefab;

    [Header("Settings")]
    [SerializeField] private float coinSpread = 3f;
    [SerializeField] private float bountyPercentage= 50f;
    [SerializeField] private int countyCoinCount = 10;
    [SerializeField] private int minBountyCoinValue = 5;
    [SerializeField] private LayerMask layerMask;
    private Collider2D[] coinBuffer = new Collider2D[1];
    private float coinRadius;

    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        coinRadius = BountyCoinPrefab.GetComponent<CircleCollider2D>().radius;

      health.OnDie += HandleDie;
    }
    
    public override void OnNetworkDespawn()
    {
        if (!IsServer) { return; }

        health.OnDie -= HandleDie;
    }

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

    private void HandleDie(Health health)
    {
      int bountyValue = (int)(TotalCoins.Value * (bountyPercentage / 100f));
        int bountyCoinValue =  bountyValue / countyCoinCount;

        if (bountyCoinValue < minBountyCoinValue)
        {
            bountyCoinValue = minBountyCoinValue;
        }

        for (int i = 0; i < countyCoinCount; i++)
        {
            BountyCoin coinInstance = Instantiate(BountyCoinPrefab, GetSpawnPoint(), Quaternion.identity);
            coinInstance.SetValue(bountyCoinValue);
            coinInstance.NetworkObject.Spawn();
        }
    }

    private Vector2 GetSpawnPoint()
    {
        float x = 0;
        float y = 0;
        while (true)
        {
            
            Vector2 spawnPoint = (Vector2)transform.position + UnityEngine.Random.insideUnitCircle * coinSpread;
            int numColliders = Physics2D.OverlapCircleNonAlloc(spawnPoint, coinRadius, coinBuffer, layerMask);
            if (numColliders == 0)
            {
                return spawnPoint;
            }
        }
    }
}
