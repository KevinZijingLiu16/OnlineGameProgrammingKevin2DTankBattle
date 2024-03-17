using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Leaderboard : NetworkBehaviour
{
    [SerializeField] private Transform leaderboardEntityHolder;
    [SerializeField] private LeaderboardEntityDisplay leaderboardEntityPrefab;
    [SerializeField] private int entitiesToDisplay = 8;

    private NetworkList<LeaderboardEntityState> leaderboardEntities;
    private List<LeaderboardEntityDisplay> entityDisplays = new List<LeaderboardEntityDisplay>();


    private void Awake()
    {
       leaderboardEntities = new NetworkList<LeaderboardEntityState>();
    }
    public override void OnNetworkSpawn()
    {
        if(IsClient)
        {
            leaderboardEntities.OnListChanged += HandleLeaderboardEntitiesChanged;
            foreach(LeaderboardEntityState entity in leaderboardEntities)
            {
               HandleLeaderboardEntitiesChanged(new NetworkListEvent<LeaderboardEntityState>
               {
                   Type = NetworkListEvent<LeaderboardEntityState>.EventType.Add,
                   
                   Value = entity
               });
            }
           
        }


        if(IsServer)
        {
            TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);
            foreach(TankPlayer player in players)
            {
                HandlePlayerSpawned(player);
            }

            TankPlayer.OnPlayerSpawned += HandlePlayerSpawned;
            TankPlayer.OnPlayerDespawned += HandlePlayerDespawned;
        }
    }


    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            leaderboardEntities.OnListChanged -= HandleLeaderboardEntitiesChanged;

        }

        if (IsServer)
        {
            TankPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
            TankPlayer.OnPlayerDespawned -= HandlePlayerDespawned;
        }
    }

    private void HandleLeaderboardEntitiesChanged(NetworkListEvent<LeaderboardEntityState> changeEvent)
    {
        switch(changeEvent.Type)
        {
            case NetworkListEvent<LeaderboardEntityState>.EventType.Add:
                Debug.Log($"[Leaderboard Add] Adding player: {changeEvent.Value.PlayerName}");
                if (!entityDisplays.Any(x => x.ClientId == changeEvent.Value.ClientId))
                {
                   LeaderboardEntityDisplay leaderboardEntity =
                        Instantiate(leaderboardEntityPrefab, leaderboardEntityHolder);
                    leaderboardEntity.Initialise(
                      changeEvent.Value.ClientId,
                       changeEvent.Value.PlayerName,
                        changeEvent.Value.Coins);
                   entityDisplays.Add(leaderboardEntity);
                }
                break;


            case NetworkListEvent<LeaderboardEntityState>.EventType.Remove:
                Debug.Log($"[Leaderboard Remove] Removing player: {changeEvent.Value.PlayerName}");
                LeaderboardEntityDisplay displayToRemove =
                    entityDisplays.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId);
                if (displayToRemove != null)
                {
                    displayToRemove.transform.SetParent(null);
                    Destroy(displayToRemove.gameObject);
                    entityDisplays.Remove(displayToRemove);
                }
               
                break;
             
            case NetworkListEvent<LeaderboardEntityState>.EventType.Value:
                Debug.Log($"[Leaderboard Value] Updating player: {changeEvent.Value.PlayerName}");
                LeaderboardEntityDisplay displayToUpdate =
                    entityDisplays.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId);
                if (displayToUpdate != null)
                {
                    displayToUpdate.UpdateCoins(changeEvent.Value.Coins);
                }
                break;

        }

        entityDisplays.Sort((x,y) => y.Coins.CompareTo(x.Coins));
        for (int i = 0; i < entityDisplays.Count; i++)
        {
            entityDisplays[i].transform.SetSiblingIndex(i);
            entityDisplays[i].UpdateText();
            bool shouldShow = i <= entitiesToDisplay - 1;
            entityDisplays[i].gameObject.SetActive(shouldShow);


        }

        LeaderboardEntityDisplay myDisplay =
        entityDisplays.FirstOrDefault(x => x.ClientId == NetworkManager.Singleton.LocalClientId);

        if (myDisplay != null)
        {
            if (myDisplay.transform.GetSiblingIndex() >= entitiesToDisplay )
            {
                leaderboardEntityHolder.GetChild(entitiesToDisplay - 1).gameObject.SetActive(false);
                myDisplay.gameObject.SetActive(true);
            }
        }
           



    }

    private void HandlePlayerSpawned(TankPlayer player)
    {
        Debug.Log($"Player spawned: {player.PlayerName.Value} with owner client ID: {player.OwnerClientId}");
        leaderboardEntities.Add(new LeaderboardEntityState
       {
           ClientId = player.OwnerClientId,
           PlayerName = player.PlayerName.Value,
           Coins = 0
       });

        player.Wallet.TotalCoins.OnValueChanged += (oldCoins,newCoins) =>
        HandlePlayerCoinsChanged(player.OwnerClientId, newCoins);
    }

    private void HandlePlayerDespawned(TankPlayer player)
    {
        if (IsServer && player.OwnerClientId == OwnerClientId) { return; }
        if (leaderboardEntities == null)
        {
            return;
        }
        foreach(LeaderboardEntityState entity in leaderboardEntities)
        {
            if(entity.ClientId != player.OwnerClientId)
            {
                continue;
            }

            leaderboardEntities.Remove(entity);
            break;
        }

        player.Wallet.TotalCoins.OnValueChanged -= (oldCoins, newCoins) =>
       HandlePlayerCoinsChanged(player.OwnerClientId, newCoins);
    }

    public void HandlePlayerCoinsChanged(ulong clientId, int newCoins)
    {
        for (int i = 0; i < leaderboardEntities.Count; i++)
        {
            if (leaderboardEntities[i].ClientId != clientId)
            {
              continue;
            }

            leaderboardEntities[i] = new LeaderboardEntityState
            {
                ClientId = clientId,
                PlayerName = leaderboardEntities[i].PlayerName,
                Coins = newCoins
            };

            return;
        }
    }
    private void LogPlayerWithMostCoins()
    {
        LeaderboardEntityState playerWithMostCoins = new LeaderboardEntityState(); // Initialize with default values
        int maxCoins = int.MinValue;

        foreach (var entity in leaderboardEntities)
        {
            if (entity.Coins > maxCoins)
            {
                maxCoins = entity.Coins;
                playerWithMostCoins = entity;
            }
        }

        if (!playerWithMostCoins.Equals(default(LeaderboardEntityState))) // Check against default value
        {
            Debug.Log($"Player with the most coins: {playerWithMostCoins.PlayerName}, Coins: {playerWithMostCoins.Coins}");
        }
        else
        {
            Debug.Log("No players in the leaderboard.");
        }
    }


}
