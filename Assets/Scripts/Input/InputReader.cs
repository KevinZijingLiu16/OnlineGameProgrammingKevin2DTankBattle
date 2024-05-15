using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static Controls;
using System;

[CreateAssetMenu(fileName = "New Input Reader", menuName = "Input/Input Reader")]

public class InputReader : ScriptableObject, IPlayerActions
{
    public event Action<bool> PrimaryFireEvent; //shooting or not shooting
    public event Action<Vector2> MoveEvent;

    public Vector2 AimPosition { get; private set; }


    private Controls controls;

    private void OnEnable()
    {
        if (controls == null)
        {
            controls = new Controls();
            controls.Player.SetCallbacks(this);
        }
        
        controls.Player.Enable();

    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnPrimaryFire(InputAction.CallbackContext context)
    {
       if (context.performed)
        {
           PrimaryFireEvent?.Invoke(true);
       }
       else if (context.canceled)
        {
           PrimaryFireEvent?.Invoke(false);
       }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        AimPosition = context.ReadValue<Vector2>();
    }
}
