using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Gate gate;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        SetGateActive(false);
    }
    public void LoseGame()
    {
        UIManager.instance.ToggleLoseGameCanvas(true);
    }

    public void WinGame()
    {
        UIManager.instance.ToggleWinGameCanvas(true);
    }
    
    public void BackToMenu()
    {
        SceneManagerScript.instance.LoadScene("Menu");
    }
    
    public void RestartGame()
    {
        SceneManagerScript.instance.LoadScene("GameplayScene");
    }

    public void SetGateActive(bool enabled)
    {
        gate.gameObject.SetActive(enabled);
    }

}
