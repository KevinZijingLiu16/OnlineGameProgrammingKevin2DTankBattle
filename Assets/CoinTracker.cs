using UnityEngine;
using System.Collections.Generic;

public class CoinTracker : MonoBehaviour
{
    private Dictionary<ulong, int> playerCoins = new Dictionary<ulong, int>();
    private Leaderboard leaderboard;

    private void Start()
    {
        // Find the Leaderboard script in the scene
        leaderboard = FindObjectOfType<Leaderboard>();

        // Subscribe to TotalCoins changes for all CoinWallet instances
        CoinWallet[] coinWallets = FindObjectsOfType<CoinWallet>();
        foreach (CoinWallet coinWallet in coinWallets)
        {
            coinWallet.TotalCoins.OnValueChanged += OnTotalCoinsChanged;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from TotalCoins changes when destroyed
        CoinWallet[] coinWallets = FindObjectsOfType<CoinWallet>();
        foreach (CoinWallet coinWallet in coinWallets)
        {
            coinWallet.TotalCoins.OnValueChanged -= OnTotalCoinsChanged;
        }
    }

    private void OnTotalCoinsChanged(int oldValue, int newValue)
    {
        // Update the total coins for the player associated with this CoinWallet
        CoinWallet coinWallet = (CoinWallet)UnityEngine.Object.FindObjectOfType(typeof(CoinWallet));
        if (coinWallet != null)
        {
            ulong clientId = coinWallet.OwnerClientId;
            playerCoins[clientId] = newValue;

            // Notify the Leaderboard script of the change
            leaderboard.HandlePlayerCoinsChanged(clientId, newValue);
        }
    }
}
