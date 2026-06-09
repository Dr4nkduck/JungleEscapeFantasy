using UnityEngine;

public class Gate : MonoBehaviour
{
    public bool isWin;
    public string nextSceneName;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (isWin)
        {
            GameManager.instance.WinGame();
        }
        else
        {
            SceneManagerScript.instance.LoadScene(nextSceneName);
        }
    }
}
