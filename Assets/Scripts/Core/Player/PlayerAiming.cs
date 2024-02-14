using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerAiming : NetworkBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform turretTransform;

    private void LateUpdate()
    {
       if (!IsOwner) return;

       Vector2 aimScreenPosition = inputReader.AimPosition;
        Vector3 aimWorldPosition = Camera.main.ScreenToWorldPoint(aimScreenPosition);

        turretTransform.up = new Vector2(aimWorldPosition.x- turretTransform.position.x, aimWorldPosition.y - turretTransform.position.y);


    }
    
}
