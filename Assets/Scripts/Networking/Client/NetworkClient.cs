using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkClient : IDisposable
{
    private NetworkManager networkManager;

    private const string menuSceneName = "MainMenu";

    public NetworkClient(NetworkManager networkManager)
    {
        this.networkManager = networkManager;

        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }


   
   
    private void OnClientDisconnect(ulong clientId)
    {
      if(clientId != 0 && clientId != networkManager.LocalClientId)
        {
            return;
        }

      if(SceneManager.GetActiveScene().name != menuSceneName)
        {
            SceneManager.LoadScene(menuSceneName);
        }

      if (networkManager.IsConnectedClient)
        {
            networkManager.Shutdown();
        }
    }


    public void Dispose()
    {
        if (networkManager != null)
        {
            networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        }
    }

}

