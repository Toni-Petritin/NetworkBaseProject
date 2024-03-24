using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x, y;
    
    private PlayerEnum owner;
    private bool hasBuilding;
    private bool currentlySelected;
    
    public void SetOwner()
    {
        
    }

    public void SetBuilding()
    {
        
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
