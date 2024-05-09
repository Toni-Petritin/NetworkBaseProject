using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkObject : NetworkBehaviour
{
    public NetworkVariable<int> PlayerMoney = new NetworkVariable<int>();
    public NetworkVariable<PlayerEnum> playerEnum = new NetworkVariable<PlayerEnum>();

    //private Dictionary<ulong, PlayerEnum> playerColors = new Dictionary<ulong, PlayerEnum>();

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
        //BoardSetup.Instance.SetPlayer((PlayerEnum)value);
    }
    
    public void ImReadyToStartGame()
    {
        playerReadied = true;
    }
    
    // [ServerRpc(RequireOwnership = false)]
    // //private void AssignPlayerColor(ulong clientId)
    // public void MyGlobalServerRpc(ServerRpcParams serverRpcParams = default)
    // {
    //     var clientId = serverRpcParams.Receive.SenderClientId;
    //     if (NetworkManager.ConnectedClients.ContainsKey(clientId))
    //     {
    //         var client = NetworkManager.ConnectedClients[clientId];
    //         // Do things for this client. Like set playerEnum.
    //     }
    // }

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

    public void BuyTerritory(int originX, int originY, int radX, int radY)
    {
        if (IsOwner)
        {
            SubmitBuyRequestServerRpc(playerEnum.Value, originX, originY, radX, radY);
        }
    }

    [Rpc(SendTo.Server)]
    void SubmitBuyRequestServerRpc(PlayerEnum player, int originX, int originY, int radX, int radY)
    {
        //BoardSetup.Instance.SetPlayer(playerEnum.Value);
        BoardSetup.Instance.SelectActionTiles(originX, originY, radX, radY);
        int cost = BoardSetup.Instance.GetSelectionCost(playerEnum.Value);
        if (cost <= PlayerMoney.Value)
        {
            BoardSetup.Instance.BuySelection(playerEnum.Value);
            PlayerMoney.Value -= cost;
            SubmitBuyRequestClientRpc(playerEnum.Value, originX, originY, radX, radY);
        }
        BoardSetup.Instance.UndoActionSelection();
    }
    
    [Rpc(SendTo.NotServer)]
    void SubmitBuyRequestClientRpc(PlayerEnum player, int originX, int originY, int radX, int radY)
    {
        BoardSetup.Instance.SelectActionTiles(originX, originY, radX, radY);
        BoardSetup.Instance.BuySelection(player);
        BoardSetup.Instance.UndoActionSelection();
    }
    
    public void BuyBuildings(int originX, int originY, int radX, int radY)
    {
        if (IsOwner)
        {
            SubmitBuildRequestServerRpc(playerEnum.Value, originX, originY, radX, radY);
        }
    }
    
    [Rpc(SendTo.Server)]
    void SubmitBuildRequestServerRpc(PlayerEnum player, int originX, int originY, int radX, int radY)
    {
        //BoardSetup.Instance.SetPlayer(playerEnum.Value);
        BoardSetup.Instance.SelectActionTiles(originX, originY, radX, radY);
        int cost = BoardSetup.Instance.GetBuildCost(player);
        if (cost <= PlayerMoney.Value)
        {
            BoardSetup.Instance.BuildOnSelection(player);
            PlayerMoney.Value -= cost;
            SubmitBuildRequestClientRpc(player, originX, originY, radX, radY);
        }
        BoardSetup.Instance.UndoActionSelection();
    }

    [Rpc(SendTo.NotServer)]
    void SubmitBuildRequestClientRpc(PlayerEnum player, int originX, int originY, int radX, int radY)
    {
        BoardSetup.Instance.SelectActionTiles(originX, originY, radX, radY);
        BoardSetup.Instance.BuildOnSelection(player);
        BoardSetup.Instance.UndoActionSelection();
    }
}
