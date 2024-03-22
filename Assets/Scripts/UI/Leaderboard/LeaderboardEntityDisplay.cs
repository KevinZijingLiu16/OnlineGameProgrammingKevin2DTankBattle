using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LeaderboardEntityDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private Color myColor;

    private FixedString32Bytes displayName;

    public ulong ClientId { get; private set; }
    public int Coins { get; private set; }

    public void Initialise(ulong clientId, FixedString32Bytes displayName, int coins)
    {
        ClientId = clientId;
        this.displayName = displayName;

        if(clientId == NetworkManager.Singleton.LocalClientId)
        {
            displayText.color = myColor;
        }

        UpdateCoins(coins);
    }

    public void UpdateCoins(int coins)
    {

        Coins = coins;

        UpdateText();
    }



    public void UpdateText()
    {
        
        displayText.text = $"#{transform.GetSiblingIndex() + 1}. Player {ClientId +1} ({Coins})";
        //displayText.text = $"#{transform.GetSiblingIndex() + 1} Tank has ({Coins}) coins";
    }

}

