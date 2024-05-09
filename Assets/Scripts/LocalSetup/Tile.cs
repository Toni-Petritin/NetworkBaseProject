using System;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x, y;

    public int cost = 1;
    
    public PlayerEnum Owner { get; private set; }
    public bool HasBuilding { get; private set; } = false;
    private bool currentlySelected;
    [SerializeField] private GameObject building;
    
    private Renderer rend;
    private Color currColor;
    
    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    public int SetOwner(PlayerEnum value)
    {
        if (value == Owner)
        {
            return 0;
        }
        
        if (value == PlayerEnum.Neutral)
        {
            rend.material.color = Color.white;
            currColor = Color.white;
        }
        else if (value == PlayerEnum.Player1)
        {
            rend.material.color = Color.blue;
            currColor = Color.blue;
        }
        else if (value == PlayerEnum.Player2)
        {
            rend.material.color = Color.red;
            currColor = Color.red;
        }
        else if (value == PlayerEnum.Player3)
        {
            rend.material.color = Color.green;
            currColor = Color.green;
        }
        return cost;
    }
    
    public bool SetBuilding(bool value, PlayerEnum player)
    {
        if (HasBuilding == value || player != Owner)
        {
            return false;
        }
        
        if (value)
        {
            cost = 10;
        }
        else
        {
            cost = 1;
        }
        
        HasBuilding = value;
        building.SetActive(value);
        return true;
    }
    
    public void Select(bool selected)
    {
        if (selected)
        {
            GetComponent<MeshRenderer>().material.color = Color.black;
        }
        else
        {
            GetComponent<MeshRenderer>().material.color = currColor;
        }
    }

    public int CostToPlayer(PlayerEnum player)
    {
        if (player != Owner)
        {
            return cost;
        }

        return 0;
    }

    public bool IsBuildableForPlayer(PlayerEnum player)
    {
        if (player == Owner && !HasBuilding)
        {
            return true;
        }
        return false;
    }
}
