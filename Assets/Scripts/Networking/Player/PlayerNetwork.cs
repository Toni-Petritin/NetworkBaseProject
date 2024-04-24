using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    public NetworkVariable<int> PlayerMoney = new NetworkVariable<int>();
    private Dictionary<ulong, PlayerEnum> playerColors = new Dictionary<ulong, PlayerEnum>();
    public NetworkVariable<PlayerEnum> playerEnum = new NetworkVariable<PlayerEnum>();
    
    void Start()
    {
        if (IsServer)
        {
            // Assign a unique PlayerEnum to each player when they connect
            // foreach (var player in NetworkManager.Singleton.ConnectedClientsList)
            // {
            //     AssignPlayerColor(player.ClientId);
            // }
        }
    }

    // Method to assign a color to a player
    private void AssignPlayerColor(ulong clientId)
    {
        // Set unique PlayerEnum to match player.
    }

    private void Update()
    {
        if (IsServer)
        {
            PlayerMoney.Value = 100;
        }
        if (IsOwner)
        {
            GetMoney();
        }
        else
        {
            // Get estimate based on own money.
        }
    }

    public void BuyTerritory(short originX, short originY, short radX, short radY)
    {
        // IsServer checks price and sets up sale with timer.
        // IsOwner asks to buy.
        // !IsOwner gets informed of sale.
    }

    public void BuyBuildings(short originX, short originY, short radX, short radY)
    {
        // IsServer checks price and sets up sale with timer.
        // IsOwner asks to buy.
        // !IsOwner gets informed of sale.
    }

    public int GetMoney()
    {
        return PlayerMoney.Value;
    }
    
}
