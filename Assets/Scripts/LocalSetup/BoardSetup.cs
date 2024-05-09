using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class BoardSetup : MonoBehaviour
{
    public static BoardSetup Instance { get; private set; }

    private const int width = 100;
    private const int height = 100;
    
    private int selX = -1, selY = -1, radX = -1, radY = -1;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI currMoneyText;
    
    // These exist because CameraLocator-script needs the values and you can't pass
    // a reference to const variables.
    public int Width { get; private set; }
    public int Height { get; private set; }
    
    private const float Padding = 0;
    
    [SerializeField] private GameObject tileObject;

    Tile[,] _tiles = new Tile[width, height];

    private List<Tile> selectionList = new();
    private List<Tile> actionSelectionList = new();

    public bool gameStarted = false;

    public int money = 0;
    //public PlayerEnum playerEnum;
    
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

        //playerEnum = PlayerEnum.Neutral;
    }
    
    void Update()
    {
        currMoneyText.text = "" + money;
        
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
                
                costText.text = "Cost: \n" + GetSelectionCost() + "/" + GetBuildCost();
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
                radX = tile.x;
                radY = tile.y;
                
                SelectTiles(selX, selY, radX, radY);
                
                costText.text = "Cost: \n" + GetSelectionCost() + "/" + GetBuildCost();
            }
            else
            {
                UndoSelection();
                selX = -1;
                selY = -1;
                radX = -1;
                radY = -1;

                costText.text = "Cost: \n" + GetSelectionCost() + "/" + GetBuildCost();
            }
        }
    }

    // public void SetPlayer(PlayerEnum player)
    // {
    //     playerEnum = player;
    // }
    
    public void SelectTiles(int originX, int originY)
    {
        UndoSelection();
        
        selectionList.Add(_tiles[originX, originY]);
        _tiles[originX,originY].Select(true);
    }
    
    // For normal selection.
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

    // For normal selection.
    public void UndoSelection()
    {
        costText.text = "Cost: \n" + 0 + "/" + 0;
        foreach (Tile tile in selectionList)
        {
            tile.Select(false);
        }
        selectionList.Clear();
    }
    
    // For actually buying and building.
    public void SelectActionTiles(int originX, int originY, int radiusX, int radiusY)
    {
        UndoActionSelection();

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
                    actionSelectionList.Add(_tiles[x, y]);
                }
            }
        }
    }

    // For actually buying and building.
    public void UndoActionSelection()
    {
        actionSelectionList.Clear();
    }

    private int GetSelectionCost()
    {
        var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        var player = playerObject.GetComponent<PlayerNetworkObject>();
        PlayerEnum playerEnum = player.playerEnum.Value;
        
        int costSum = 0;
        foreach (Tile tile in selectionList)
        {
            costSum += tile.CostToPlayer(playerEnum);
        }
        
        return costSum;
    }
    
    public int GetSelectionCost(PlayerEnum player)
    {
        int costSum = 0;
        foreach (Tile tile in actionSelectionList)
        {
            costSum += tile.CostToPlayer(player);
        }
        
        return costSum;
    }

    public int BuySelection(PlayerEnum player)
    {
        int costSum = 0;
        foreach (Tile tile in actionSelectionList)
        {
            costSum += tile.SetOwner(player);
        }

        return costSum;
    }

    private int GetBuildCost()
    {
        var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        var player = playerObject.GetComponent<PlayerNetworkObject>();
        PlayerEnum playerEnum = player.playerEnum.Value;
        
        int number = 0;
        foreach (Tile tile in selectionList)
        {
            if (tile.IsBuildableForPlayer(playerEnum))
            {
                number++;
            }
        }
        number *= 10;
        return number;
    }
    
    public int GetBuildCost(PlayerEnum player)
    {
        int number = 0;
        foreach (Tile tile in actionSelectionList)
        {
            if (tile.IsBuildableForPlayer(player))
            {
                number++;
            }
        }
        number *= 10;
        return number;
    }
    
    public int BuildOnSelection(PlayerEnum player)
    {
        int number = 0;
        foreach (Tile tile in actionSelectionList)
        {
            if (tile.SetBuilding(true, player))
            {
                number++;
            }
        }
        number *= 10;
        return number;
    }

    public void BuyButton()
    {
        if (selX < 0 || selY < 0 || radX < 0 || radY < 0)
        {
            return;
        }
        
        if (NetworkManager.Singleton.IsClient)
        {
            var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
            var player = playerObject.GetComponent<PlayerNetworkObject>();
            player.BuyTerritory(selX, selY, radX, radY);
        }
    }
    
    public void BuildButton()
    {
        if (selX < 0 || selY < 0 || radX < 0 || radY < 0)
        {
            return;
        }

        if (NetworkManager.Singleton.IsClient)
        {
            var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
            var player = playerObject.GetComponent<PlayerNetworkObject>();
            player.BuyBuildings(selX, selY, radX, radY);
        }
    }
}
