using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton để các script khác truy cập GameManager
    public static GameManager instance;
    public Checkpoint checkpoint = null;

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
        if (checkpoint == null)
        {
            SceneManagerScript.instance.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            PlayerController player = FindAnyObjectByType<PlayerController>();

            if (player != null)
            {
                player.Respawn(checkpoint.playerPos, checkpoint.playerHealth);
            }

            if (UIManager.instance != null)
            {
                UIManager.instance.ToggleLoseGameCanvas(false);
            }
        }
    }

    public void SetCheckPoint(Checkpoint checkpoint)
    {
        this.checkpoint = checkpoint;
    }

    // Bật hoặc tắt Gate
    public void SetGateActive(bool enabled)
    {
        gate.gameObject.SetActive(enabled);
    }
}