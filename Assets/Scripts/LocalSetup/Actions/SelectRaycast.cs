using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using TMPro;
using UnityEngine;

public class SelectRaycast : MonoBehaviour
{

    private int selX = -1, selY = -1, radX = -1, radY = -1;
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    
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
                
                BoardSetup.Instance.SelectTiles(tile.x, tile.y);
                selX = tile.x;
                selY = tile.y;
                
                textMeshProUGUI.text = "Cost: " + BoardSetup.Instance.GetSelectionCost();
            }
            //else
            //{
            //    BoardSetup.Instance.UndoSelection();
            //    selX = -1;
            //    selY = -1;
                
            //    textMeshProUGUI.text = "Cost: " + BoardSetup.Instance.GetSelectionCost();
            //}
        }
        
        if (Input.GetMouseButtonDown(1)) // Right mouse button
        {
            // Cast a ray from the mouse position into the scene
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if the ray hits any objects
            if (Physics.Raycast(ray, out hit) && selX != -1 && selY != -1)
            {
                Tile tile = hit.collider.gameObject.GetComponent<Tile>();
                
                BoardSetup.Instance.SelectTiles(selX, selY, tile.x, tile.y);
                
                textMeshProUGUI.text = "Cost: " + BoardSetup.Instance.GetSelectionCost();
            }
            else
            {
                BoardSetup.Instance.UndoSelection();
                selX = -1;
                selY = -1;

                textMeshProUGUI.text = "Cost: " + BoardSetup.Instance.GetSelectionCost();
            }
        }
    }
}
