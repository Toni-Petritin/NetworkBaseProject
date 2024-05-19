using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkTesting : MonoBehaviour
{
    public UnityTransport transport;
    private float timer = 0f;
    private float lossDuration = 10f; // Duration of simulated connection loss in seconds
    private bool simulatingLoss = false;

    // void Start()
    // {
    //     transport = FindObjectOfType<UnityTransport>();
    //     if (transport == null)
    //     {
    //         Debug.LogError("UnityTransport component not found!");
    //     }
    // }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) // Press 'L' to toggle connection loss
        {
            ToggleConnectionLoss();
        }

        if (simulatingLoss)
        {
            timer += Time.deltaTime;
            if (timer >= lossDuration)
            {
                EndConnectionLoss();
            }
        }
    }

    void ToggleConnectionLoss()
    {
        if (!simulatingLoss)
        {
            StartConnectionLoss();
        }
        else
        {
            EndConnectionLoss();
        }
    }

    void StartConnectionLoss()
    {
        simulatingLoss = true;
        timer = 0f;

        // Enable the debug simulator
        transport.DebugSimulator.PacketDropRate = 100; // Drop all packets

        Debug.Log("Simulating connection loss...");
    }

    void EndConnectionLoss()
    {
        simulatingLoss = false;

        // Disable the debug simulator
        transport.DebugSimulator.PacketDropRate = 10;

        Debug.Log("Connection restored.");
    }
}
