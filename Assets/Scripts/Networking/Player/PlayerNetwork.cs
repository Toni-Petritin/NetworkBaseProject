using System.Collections.Generic;
using Unity.Netcode;

public class PlayerNetwork : NetworkBehaviour
{
    public NetworkVariable<int> PlayerMoney = new NetworkVariable<int>();
    public NetworkVariable<PlayerEnum> playerEnum = new NetworkVariable<PlayerEnum>();

    private Dictionary<ulong, PlayerEnum> playerColors = new Dictionary<ulong, PlayerEnum>();

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
        // IsOwner asks to buy. Makes an rpc request.
        // !IsOwner gets informed of sale.
    }

    public void BuyBuildings(short originX, short originY, short radX, short radY)
    {
        // IsServer checks price and sets up sale with timer.
        // IsOwner asks to buy. Mkes an rpc request.
        // !IsOwner gets informed of sale.
    }

    public int GetMoney()
    {
        return PlayerMoney.Value;
    }
    
}
