using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkObject : NetworkBehaviour
{
    public NetworkVariable<int> PlayerMoney = new NetworkVariable<int>();
    public NetworkVariable<PlayerEnum> playerEnum = new NetworkVariable<PlayerEnum>();

    private Dictionary<ulong, PlayerEnum> playerColors = new Dictionary<ulong, PlayerEnum>();

    public bool playerReadied = false;

    private NetworkVariable<float> timer = new NetworkVariable<float>();
    
    // void Start()
    // {
    //     if (IsServer)
    //     {
    //         Assign a unique PlayerEnum to each player when they connect
    //         foreach (var player in NetworkManager.Singleton.ConnectedClientsList)
    //         {
    //             AssignPlayerColor(player.ClientId);
    //         }
    //     }
    // }

    public void ServerStartedGame(int value)
    {
        BoardSetup.Instance.gameStarted = true;
        playerEnum.Value = (PlayerEnum)value;
    }

    public void ImReadyToStartGame()
    {
        playerReadied = true;
    }
    
    [ServerRpc(RequireOwnership = false)]
    //private void AssignPlayerColor(ulong clientId)
    public void MyGlobalServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        if (NetworkManager.ConnectedClients.ContainsKey(clientId))
        {
            var client = NetworkManager.ConnectedClients[clientId];
            // Do things for this client. Like set playerEnum.
        }
    }

    private void Update()
    {
        if (IsServer)
        {
            timer.Value += Time.deltaTime;
            if (timer.Value >= 1)
            {
                timer.Value = 0;
                PlayerMoney.Value += 100;
            }
        }
        if (IsOwner)
        {
            BoardSetup.Instance.money = PlayerMoney.Value;
        }
    }

    public void BuyTerritory(short originX, short originY, short radX, short radY)
    {
        // IsServer checks price and sets up sale with timer.
        // IsOwner asks to buy. Makes an rpc request.
        // !IsOwner gets informed of sale.
        if (IsOwner)
        {
            SubmitBuyRequestServerRpc(originX, originY, radX, radY);
        }
    }

    [Rpc(SendTo.Server)]
    void SubmitBuyRequestServerRpc(short originX, short originY, short radX, short radY)
    {
        BoardSetup.Instance.SetPlayer(playerEnum.Value);
        BoardSetup.Instance.SelectTiles(originX, originY, radX, radY);
        int cost = BoardSetup.Instance.GetBuildCost(playerEnum.Value);
        if (cost <= PlayerMoney.Value)
        {
            BoardSetup.Instance.BuySelection(playerEnum.Value);
            PlayerMoney.Value -= cost;
        }
        BoardSetup.Instance.UndoSelection();
    }
    
    [ClientRpc]
    void SubmitBuyRequestClientRpc(PlayerEnum player, short originX, short originY, short radX, short radY)
    {
        
    }
    
    public void BuyBuildings(short originX, short originY, short radX, short radY)
    {
        // IsServer checks price and sets up sale with timer.
        // IsOwner asks to buy. Mkes an rpc request.
        // !IsOwner gets informed of sale.
        if (IsOwner)
        {
            SubmitBuildRequestServerRpc(originX, originY, radX, radY);
        }
    }
    
    [Rpc(SendTo.Server)]
    void SubmitBuildRequestServerRpc(short originX, short originY, short radX, short radY)
    {
        BoardSetup.Instance.SetPlayer(playerEnum.Value);
        BoardSetup.Instance.SelectTiles(originX, originY, radX, radY);
        int cost = BoardSetup.Instance.GetBuildCost(playerEnum.Value);
        if (cost <= PlayerMoney.Value)
        {
            BoardSetup.Instance.BuildOnSelection(playerEnum.Value);
            PlayerMoney.Value -= cost;
        }
        BoardSetup.Instance.UndoSelection();
    }

    [ClientRpc]
    void SubmitBuildRequestClientRpc(PlayerEnum player, short originX, short originY, short radX, short radY)
    {
        
    }
}
