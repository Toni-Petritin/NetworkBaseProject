using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class BoardSetup : MonoBehaviour
{
    public static BoardSetup Instance { get; private set; }

    private const int width = 100;
    private const int height = 100;
    
    private int selX = -1, selY = -1, radX = -1, radY = -1;
    [SerializeField] private TextMeshProUGUI costText;
    
    // These exist because CameraLocator-script needs the values and you can't pass
    // a reference to const variables.
    public int Width { get; private set; }
    public int Height { get; private set; }
    
    private const float Padding = 0;
    
    [SerializeField] private GameObject tileObject;

    Tile[,] _tiles = new Tile[width, height];

    public List<Tile> selectionList = new();

    public bool gameStarted = true;
    
    private void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        Width = width;
        Height = height;
    }

    private void Start()
    {
        for(int x = 0; x < width; x++)
        {
            for( int y = 0; y < height; y++)
            {
                GameObject temp =Instantiate(tileObject,
                    new Vector3(x * (tileObject.transform.localScale.x + Padding), 0, y * (tileObject.transform.localScale.y + Padding)),
                    tileObject.transform.rotation);
                // I didn't want to go looking for the Tile-component all the time.
                // So, I changed the tiles to be Tile type here.
                _tiles[x, y] = temp.GetComponent<Tile>();
                _tiles[x, y].x = x;
                _tiles[x, y].y = y;
            }
        }
    }
    
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0) && gameStarted) // Left mouse button
        {
            // Cast a ray from the mouse position into the scene
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if the ray hits any objects
            if (Physics.Raycast(ray, out hit))
            {
                Tile tile = hit.collider.gameObject.GetComponent<Tile>();
                
                SelectTiles(tile.x, tile.y);
                selX = tile.x;
                selY = tile.y;
                
                costText.text = "Cost: " + GetSelectionCost();
            }
            //else
            //{
            //    BoardSetup.Instance.UndoSelection();
            //    selX = -1;
            //    selY = -1;
                
            //    textMeshProUGUI.text = "Cost: " + BoardSetup.Instance.GetSelectionCost();
            //}
        }
        
        if (Input.GetMouseButtonDown(1) && gameStarted) // Right mouse button
        {
            // Cast a ray from the mouse position into the scene
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if the ray hits any objects
            if (Physics.Raycast(ray, out hit) && selX != -1 && selY != -1)
            {
                Tile tile = hit.collider.gameObject.GetComponent<Tile>();
                
                SelectTiles(selX, selY, tile.x, tile.y);
                
                costText.text = "Cost: " + GetSelectionCost();
            }
            else
            {
                UndoSelection();
                selX = -1;
                selY = -1;

                costText.text = "Cost: " + GetSelectionCost();
            }
        }
    }
    
    public void SelectTiles(int originX, int originY)
    {
        UndoSelection();
        
        selectionList.Add(_tiles[originX, originY]);
        _tiles[originX,originY].Select(true);
    }
    
    public void SelectTiles(int originX, int originY, int radiusX, int radiusY)
    {
        UndoSelection();

        int sqrtRadius = (originX - radiusX)*(originX - radiusX) + (originY - radiusY)*(originY - radiusY);
        float radius = Mathf.Sqrt(sqrtRadius);
        
        Debug.Log("Radius: " + radius);
        for (int x = Mathf.Max(0, originX-(int)radius); x <= Mathf.Min(width - 1, originX + (int)radius); x++)
        {
            for (int y = Mathf.Max(0, originY - (int)radius); y <= Mathf.Min(width - 1, originY + (int)radius); y++)
            {
                int sqrtDist = (originX - x)*(originX - x) + (originY - y)*(originY - y);
                if (sqrtRadius >= sqrtDist)
                {
                    selectionList.Add(_tiles[x, y]);
                    _tiles[x,y].Select(true);
                }
            }
        }
    }

    public void UndoSelection()
    {
        foreach (Tile tile in selectionList)
        {
            tile.Select(false);
        }
        selectionList.Clear();
    }

    public int GetSelectionCost()
    {
        int sum = 0;
        foreach (Tile tile in selectionList)
        {
            sum += tile.cost;
        }
        
        return sum;
    }

    public void BuySelection()
    {
        int number = 0;
        foreach (Tile tile in selectionList)
        {
            number++;
        }
        Debug.Log("Bought " + number + " tiles.");
    }

    public void BuildOnSelection()
    {
        int number = 0;
        foreach (Tile tile in selectionList)
        {
            tile.SetBuilding(true);
            number++;
        }
        Debug.Log("Built on " + number + " tiles.");
    }
}
