using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton để các script khác truy cập GameManager
    public static GameManager instance;

    // Tham chiếu đến Gate trong scene
    public Gate gate;

    void Awake()
    {
        // Đảm bảo chỉ có một GameManager duy nhất
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        // Khi bắt đầu game, ẩn Gate
        SetGateActive(false);
    }

    // Hiển thị màn hình thua
    public void LoseGame()
    {
        UIManager.instance.ToggleLoseGameCanvas(true);
    }

    // Hiển thị màn hình thắng
    public void WinGame()
    {
        UIManager.instance.ToggleWinGameCanvas(true);
    }

    // Quay về menu chính
    public void BackToMenu()
    {
        SceneManagerScript.instance.LoadScene("Menu");
    }

    // Chơi lại màn hiện tại
    public void RestartGame()
    {
        SceneManagerScript.instance.LoadScene("GameplayScene");
    }

    // Bật hoặc tắt Gate
    public void SetGateActive(bool enabled)
    {
        gate.gameObject.SetActive(enabled);
    }
}