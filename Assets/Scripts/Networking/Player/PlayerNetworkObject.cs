using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkObject : NetworkBehaviour
{
    public NetworkVariable<int> PlayerMoney = new NetworkVariable<int>();
    public NetworkVariable<PlayerEnum> playerEnum = new NetworkVariable<PlayerEnum>();

    public NetworkVariable<bool> playerReadied = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> gameStarted = new NetworkVariable<bool>(false);
    
    private NetworkVariable<float> timer = new NetworkVariable<float>();
    
    public void ServerStartedGame(int value)
    {
        playerEnum.Value = (PlayerEnum)value;
        if (IsServer)
        {
            gameStarted.Value = true;
            SubmitStartGameServerRpc(value);
        }
    }

    [Rpc(SendTo.NotServer)]
    void SubmitStartGameServerRpc(int value)
    {
        BoardSetup.Instance.gameStarted = true;
    }
    
    public void ImReadyToStartGame()
    {
        if (IsOwner)
        {
            SubmitReadyCheckServerRpc();
        }
    }

    [Rpc(SendTo.Server)]
    void SubmitReadyCheckServerRpc()
    {
        playerReadied.Value = true;
    }
    
    private void Update()
    {
        if (IsServer && gameStarted.Value)
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
            // This is probably not necessary at this point, but it would be useful if we decided to do interpolation on value.
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
            BoardSetup.Instance.gameActionArray[BoardSetup.Instance.executedIndex + 1] = new GameAction(player, true, originX, originY, radX, radY);
            BoardSetup.Instance.executedIndex++;
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
            BoardSetup.Instance.gameActionArray[BoardSetup.Instance.executedIndex + 1] = new GameAction(player, false, originX, originY, radX, radY);
            BoardSetup.Instance.executedIndex++;
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

    public void GetMissingIndexAction(int index)
    {
        if (IsOwner)
        {
            SubmitIndexServerRpc(index);
        }
    }

    [Rpc(SendTo.Server)] 
    void SubmitIndexServerRpc(int index)
    {
        GameAction requestedAction = BoardSetup.Instance.gameActionArray[index];
    }
}
