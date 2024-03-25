using System.Collections.Generic;
using UnityEngine;

public class BoardSetup : MonoBehaviour
{
    public static BoardSetup Instance { get; private set; }

    private const int width = 50;
    private const int height = 50;
    
    // These exist because CameraLocator-script needs the values and you can't pass
    // a reference to const variables.
    public int Width { get; private set; }
    public int Height { get; private set; }
    
    private const float Padding = 0;
    
    [SerializeField] private GameObject tileObject;

    Tile[,] _tiles = new Tile[width, height];

    private List<Tile> _selectionList = new();

    private void Awake()
    {
        Width = width;
        Height = height;
        
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
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

    public void SelectTiles(int originX, int originY)
    {
        foreach(Tile tile in _selectionList)
        {
            tile.Select(false);
        }
        _selectionList.Clear();
        _selectionList.Add(_tiles[originX, originY]);
        _tiles[originX,originY].Select(true);
    }
    
    public void SelectTiles(int originX, int originY, int radiusX, int radiusY)
    {
        foreach (Tile tile in _selectionList)
        {
            tile.Select(false);
        }
        _selectionList.Clear();

        _tiles[originX,originY].Select(true);
    }
}
