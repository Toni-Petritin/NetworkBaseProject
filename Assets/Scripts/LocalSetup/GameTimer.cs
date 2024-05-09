using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;

    public int hour;
    public int minutes;
    public int seconds;
    
    void Start()
    {
        hour = System.DateTime.Now.Hour;
        minutes = System.DateTime.Now.Minute;
        seconds = System.DateTime.Now.Second;
        textMeshProUGUI.text = hour + ":" + minutes + ":" + seconds;
    }

    void Update()
    {
        hour = System.DateTime.Now.Hour;
        minutes = System.DateTime.Now.Minute;
        seconds = System.DateTime.Now.Second;
        textMeshProUGUI.text = hour + ":" + minutes + ":" + seconds;
    }
}
