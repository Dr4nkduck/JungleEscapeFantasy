using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    // Canvas hiển thị khi người chơi thua
    public GameObject LoseGameCanvas;

    // Canvas hiển thị khi người chơi thắng
    public GameObject WinGameCanvas;

    // Mảng Text hiển thị số coin trên các màn hình UI
    public TextMeshProUGUI[] coinTexts;

    // Mảng Text hiển thị số high score trên các màn hình UI
    public TextMeshProUGUI[] highScoreTexts;

    // Singleton để các script khác có thể truy cập UIManager
    public static UIManager instance;

    void Awake()
    {
        // Đảm bảo chỉ có một instance của UIManager
        if (instance == null)
        {
            instance = this;
        }
    }

    // Cập nhật số coin trên tất cả các Text trong mảng coinTexts
    public void UpdateCoinText(int points)
    {
        foreach (var i in coinTexts)
            i.text = points.ToString();
    }

    // Cập nhật số high score trên tất cả các Text trong mảng highScoreTexts
    public void UpdateHighScoreText(int highScore)
    {
        if (highScoreTexts == null || highScoreTexts.Length == 0)
        {
            FindHighScoreTexts();
        }

        if (highScoreTexts == null)
        {
            return;
        }

        UpdateAllHighScoreTexts(highScore);
    }

    private void UpdateAllHighScoreTexts(int highScore)
    {
        foreach (var t in highScoreTexts)
        {
            if (t == null) continue;
            t.text = highScore.ToString();
        }
    }

    private void FindHighScoreTexts()
    {
        var allTexts = FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        highScoreTexts = System.Array.FindAll(allTexts, t => t.gameObject.name.ToLower().Contains("highscore"));
    }

    // Bật hoặc tắt màn hình thua game
    public void ToggleLoseGameCanvas(bool enabled)
    {
        LoseGameCanvas.SetActive(enabled);
    }

    // Bật hoặc tắt màn hình thắng game
    public void ToggleWinGameCanvas(bool enabled)
    {
        WinGameCanvas.SetActive(enabled);
    }
}