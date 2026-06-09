using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject LoseGameCanvas;
    public GameObject WinGameCanvas;
    public TextMeshProUGUI[] coinTexts;

    public static UIManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void UpdateCoinText(int points)
    {
        foreach(var i in coinTexts) i.text = points.ToString();
    }

    public void ToggleLoseGameCanvas(bool enabled)
    {
        LoseGameCanvas.SetActive(enabled);
    }
    public void ToggleWinGameCanvas(bool enabled)
    {
        WinGameCanvas.SetActive(enabled);
    }

}