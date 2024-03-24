using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectRaycast : MonoBehaviour
{
    void Update()
    {
        // TODO: Shift + LMB should select a second tile.
        
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            // Cast a ray from the mouse position into the scene
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if the ray hits any objects
            if (Physics.Raycast(ray, out hit))
            {
                Tile tile = hit.collider.gameObject.GetComponent<Tile>();
                
                BoardSetup.Instance.SelectTiles(tile.x, tile.y);
                
                Debug.Log("Selected Tile: (" + tile.x + "," + tile.y + ")");
            }
        }
        
    }
}
