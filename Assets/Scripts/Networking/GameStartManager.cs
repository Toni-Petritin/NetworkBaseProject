using Unity.Netcode;
using UnityEngine;

public class GameStartManager : MonoBehaviour
{
    private static bool readyUIOn = true;
    private static int playersReady = 0;
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(250, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            StartButtons();
        }
        else if (readyUIOn /*&& NetworkManager.Singleton.IsServer*/)
        {
            StatusLabels();
            StartGame();
        }
        else
        {
            StatusLabels();
        }

        GUILayout.EndArea();
    }
    
    static void StartButtons()
    {
        if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
        if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
        if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
    }

    static void StatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ?
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        // GUILayout.Label("Transport: " + NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
    }
    
    static void StartGame()
    {
        // There is no UI change on every frame, because it's just tedium - it would follow similar logic as here.
        if (GUILayout.Button(NetworkManager.Singleton.IsServer ? "Start game (" + playersReady + " players ready)" : "Ready to start game"))
        {
            if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
            {
                playersReady = 0;
                int playerEnum = 1;
                bool allClientsReady = true;
                foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
                {
                    if (NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<PlayerNetworkObject>().playerReadied.Value)
                    {
                        playersReady++;
                    }
                    else
                    {
                        allClientsReady = false;
                    }
                }
                // Game only starts if all players readied and their number is between 1-3.
                // There are no checks after this, so expectation is that joining clients after this have no agency
                // and function like "spectators".
                if (playersReady is > 0 and <= 3 && allClientsReady)
                {
                    readyUIOn = false;
                    foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
                    {
                        NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<PlayerNetworkObject>().ServerStartedGame(playerEnum);
                        playerEnum++;
                    }
                }
            }
            else
            {
                var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
                var player = playerObject.GetComponent<PlayerNetworkObject>();
                player.ImReadyToStartGame();
                readyUIOn = false;
            }
        }
    }
}
