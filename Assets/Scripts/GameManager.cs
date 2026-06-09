using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void LoseGame()
    {
        Debug.Log("You lose!");
    }

    public void WinGame()
    {
        Debug.Log("You win!");
    }
    
    public void BackToMenu()
    {
        SceneManagerScript.instance.LoadScene("Menu");
    }
    
    public void RestartGame()
    {
        SceneManagerScript.instance.LoadScene("Game");
    }

}
