using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    // Singleton để các script khác có thể truy cập SceneManagerScript
    public static SceneManagerScript instance;

    void Awake()
    {
        // Đảm bảo chỉ có một instance duy nhất
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        // Chưa sử dụng
    }

    // Chuyển sang scene khác theo tên scene
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Thoát game
    public void ExitGame()
    {
        Application.Quit();
    }
}