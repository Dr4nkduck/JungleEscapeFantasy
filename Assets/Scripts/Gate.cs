using UnityEngine;

public class Gate : MonoBehaviour
{
    // Đánh dấu đây có phải cổng chiến thắng hay không
    public bool isWin;

    // Tên scene sẽ chuyển tới nếu không phải cổng chiến thắng
    public string nextSceneName;

    // Được gọi khi có đối tượng đi vào vùng Trigger của Gate
    void OnTriggerEnter2D(Collider2D other)
    {
        // Nếu đây là cổng thắng game
        if (isWin && other.tag == "Player")
        {
            // Gọi hàm thắng game trong GameManager
            other.GetComponent<PlayerController>().MoveAble = false;
            GameManager.instance.WinGame();
        }
        else
        {
            // Chuyển sang scene tiếp theo
            SceneManagerScript.instance.LoadScene(nextSceneName);
        }
    }
}