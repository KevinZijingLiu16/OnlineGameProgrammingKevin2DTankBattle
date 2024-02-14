using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DealDamage : MonoBehaviour
{
    [SerializeField] private int damage = 5;

    private ulong ownerClientId;

    public void SetOwner(ulong ownerClientId)
    {
        this.ownerClientId = ownerClientId;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.attachedRigidbody == null) { return; }

        if (collision.attachedRigidbody.TryGetComponent<NetworkObject>(out NetworkObject netObj))
        {
            if (ownerClientId == netObj.OwnerClientId)
            {
                return;
            }
        }

        if (collision.attachedRigidbody.TryGetComponent<Health>(out Health health))
        {
            health.TakeDamage(damage);
        }
    }

}
