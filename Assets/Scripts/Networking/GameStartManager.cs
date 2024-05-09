using Unity.Netcode;
using UnityEngine;

public class GameStartManager : MonoBehaviour
{
    private static bool readyUIOn = true;
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(250, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            StartButtons();
        }
        else if (readyUIOn && NetworkManager.Singleton.IsServer)
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
        if (GUILayout.Button(NetworkManager.Singleton.IsServer ? "Start game" : "Ready to start game"))
        {
            if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
            {
                bool playersReady = true;
                int playerEnum = 1;
                // foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
                // {
                //     if (!NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<PlayerNetworkObject>().playerReadied)
                //     {
                //         playersReady = false;
                //     }
                //     
                // }
                // if (playersReady)
                // {
                //     readyUIOn = false;
                //     foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
                //     {
                //         NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<PlayerNetworkObject>().ServerStartedGame(playerEnum);
                //         playerEnum++;
                //     }
                // }
                readyUIOn = false;
                foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
                {
                    NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<PlayerNetworkObject>().ServerStartedGame(playerEnum);
                    playerEnum++;
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
