using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI coinText;

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
        coinText.text = points.ToString();
    }

}