using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectRaycast : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            // Cast a ray from the mouse position into the scene
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if the ray hits any objects
            if (Physics.Raycast(ray, out hit))
            {
                Tile tile = hit.collider.gameObject.GetComponent<Tile>();
                
                
                // Do something with the selected object
                Debug.Log("Selected Tile: (" + tile.x + "," + tile.y + ")");
            }
        }
    }
}
