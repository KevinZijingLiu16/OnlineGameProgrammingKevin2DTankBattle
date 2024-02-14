using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ConnectionButton : MonoBehaviour
{
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }
    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
