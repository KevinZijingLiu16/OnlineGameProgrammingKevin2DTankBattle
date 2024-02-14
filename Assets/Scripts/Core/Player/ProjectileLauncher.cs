using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using System.Runtime.CompilerServices;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private CoinWallet coinWallet;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject serverProjectilePrefab;
    [SerializeField] private GameObject clientProjectilePrefab;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private Collider2D playerCollider;

    [Header("Settings")]
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float fireRate;
    [SerializeField] private float muzzleFlashDuration;
    [SerializeField] private int costToFire;


    private bool shouldFire;
    private float fireRateTimer;
   
    private float muzzleFlashTimer;

    public override void OnNetworkSpawn()
    {
       if (!IsOwner)
        {
            return;
        }

       inputReader.PrimaryFireEvent += HandlePrimaryFire;

       
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
        {
            return;
        }

        inputReader.PrimaryFireEvent -= HandlePrimaryFire;


    }

    // Update is called once per frame
    void Update()
    {
        if (muzzleFlashTimer > 0)
        {
            muzzleFlashTimer -= Time.deltaTime;
            if (muzzleFlashTimer <= 0)
            {
                muzzleFlash.SetActive(false);
            }
        }

        if (!IsOwner)
        {
            return;
        }

        if (fireRateTimer > 0)
        {
            fireRateTimer -= Time.deltaTime;
        }

        if (!shouldFire)
        {
            return;
        }

        if (fireRateTimer > 0)
        {
            return;
        }
        

        if (coinWallet.TotalCoins.Value < costToFire)
        {
            return;
        }

        PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);
        SpawnProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);

        fireRateTimer = 1 / fireRate;
    }

    [ServerRpc]
    private void PrimaryFireServerRpc(Vector3 spawnPosition, Vector3 direction)
    {
        if (coinWallet.TotalCoins.Value < costToFire)
        {
            return;
        }
        coinWallet.SpendCoins(costToFire);



        GameObject projectileInstance = Instantiate(serverProjectilePrefab, spawnPosition, Quaternion.identity);

        projectileInstance.transform.up = direction;

        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());

        if (projectileInstance.TryGetComponent<DealDamage>(out DealDamage dealDamage))
        {
            dealDamage.SetOwner(OwnerClientId);
        }


        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * projectileSpeed;
        }

        PrimaryFireClientRpc(spawnPosition, direction);
    }
    [ClientRpc]
    private void PrimaryFireClientRpc(Vector3 spawnPosition, Vector3 direction)
    {
       if (IsOwner)
        {
            return;
        }

        SpawnProjectile(spawnPosition, direction);
    }

    private void SpawnProjectile(Vector3 spawnPosition, Vector3 direction)
    {
        muzzleFlash.SetActive(true);
        muzzleFlashTimer = muzzleFlashDuration;

       GameObject projectileInstance = Instantiate(clientProjectilePrefab, spawnPosition, Quaternion.identity);

        projectileInstance.transform.up = direction;

        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());

        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * projectileSpeed;
        }
    }

    private void HandlePrimaryFire(bool shouldFire)
    {
       this.shouldFire = shouldFire;
    }
}
