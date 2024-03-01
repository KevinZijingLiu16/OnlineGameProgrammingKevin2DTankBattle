using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{
    private static ClientSingleton instance; // Singleton instance, lowercase 

    public ClientGameManager GameManager { get; private set; }

    public static ClientSingleton Instance // Singleton accessor ; this is a property
    {
        get
        {
            if (instance != null) { return instance; }

            instance = FindObjectOfType<ClientSingleton>(); 

            if (instance == null) // If still can't find the instance, log an error
            {
                Debug.LogError("No ClientSingleton in the scene!");
                return null;
            }

            return instance;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public async Task<bool> CreateClient() //async method
    {
        GameManager = new ClientGameManager();

        return await GameManager.InitAsync(); //wait for the method to complete
    }

    private void OnDestroy()
    {
        GameManager?.Dispose();
    }
}
