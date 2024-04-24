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
    
    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    public void SetOwner(PlayerEnum value)
    {
        if (value == PlayerEnum.Neutral)
        {
            rend.material.color = Color.white;
        }
        else if (value == PlayerEnum.Player1)
        {
            rend.material.color = Color.blue;
        }
        else if (value == PlayerEnum.Player2)
        {
            rend.material.color = Color.red;
        }
        else if (value == PlayerEnum.Player3)
        {
            rend.material.color = Color.green;
        }
        
    }
    
    public void SetBuilding(bool value)
    {
        HasBuilding = value;
        building.SetActive(building);
    }
    
    public void Select(bool selected)
    {
        if (selected)
        {
            GetComponent<MeshRenderer>().material.color = Color.black;
        }
        else
        {
            GetComponent<MeshRenderer>().material.color = Color.white;
        }
    }
    
}
