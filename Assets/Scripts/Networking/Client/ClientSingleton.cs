using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{
    private static ClientSingleton instance; // Singleton instance, lowercase 

    private ClientGameManager gameManager;

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

    public async Task CreateClient() //async method
    {
        gameManager = new ClientGameManager();

        await gameManager.InitAsync(); //wait for the method to complete
    }
}
