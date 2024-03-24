using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class RpcTest : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsServer && IsOwner) //Only send an RPC to the server on the client that owns the NetworkObject that owns this NetworkBehaviour instance
        {
            // Lightsail server endpoint
            string serverURL = "51.20.125.76";
            StartCoroutine(SendRequestCoroutine(serverURL));
            
            // Original code:
            // TestServerRpc(0, NetworkObjectId);
        }
    }
    
    IEnumerator SendRequestCoroutine(string serverURL)
    {
        // Sending HTTP GET request to the server
        UnityWebRequest request = UnityWebRequest.Get(serverURL);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to send request: " + request.error);
        }
        else
        {
            Debug.Log("Server response: " + request.downloadHandler.text);
        }
    }
    

    [Rpc(SendTo.ClientsAndHost)]
    void TestClientRpc(int value, ulong sourceNetworkObjectId)
    {
        Debug.Log($"Client Received the RPC #{value} on NetworkObject #{sourceNetworkObjectId}");
        if (IsOwner) //Only send an RPC to the server on the client that owns the NetworkObject that owns this NetworkBehaviour instance
        {
            TestServerRpc(value + 1, sourceNetworkObjectId);
        }
    }

    [Rpc(SendTo.Server)]
    void TestServerRpc(int value, ulong sourceNetworkObjectId)
    {
        Debug.Log($"Server Received the RPC #{value} on NetworkObject #{sourceNetworkObjectId}");
        TestClientRpc(value, sourceNetworkObjectId);
    }
}