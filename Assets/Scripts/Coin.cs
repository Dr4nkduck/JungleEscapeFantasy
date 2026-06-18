using UnityEngine;

public class Coin : MonoBehaviour
{
    // --- CÁC BIẾN CẤU HÌNH ---
    public AudioClip audioClip;   // Âm thanh sẽ phát ra khi đồng xu này được nhặt (ví dụ: tiếng "Ting")
    public int pointValue = 1;     // Giá trị điểm của đồng xu này (mặc định là 1 điểm, có thể chỉnh trong Inspector)

    /// <summary>
    /// Hàm này tự động chạy khi có một đối tượng khác đi vào vùng va chạm (Trigger) của đồng xu.
    /// </summary>
    /// <param name="other">Thông tin của đối tượng va chạm với đồng xu</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        // In ra dòng "Enter" ở tab Console để lập trình viên kiểm tra xem va chạm có hoạt động hay không
        Debug.Log("Enter");

        // Kiểm tra xem đối tượng va chạm có Tag là "Player" (Người chơi) hay không
        if (other.gameObject.tag == "Player")
        {
            // 1. Gọi đến CoinManager để cộng thêm điểm (truyền thông tin đồng xu này và giá trị điểm của nó)
            CoinManager.instance.AddPoint(this, pointValue);

            // 2. Gọi đến AudioManager để phát đoạn âm thanh ăn xu (audioClip) đã kéo thả ở Inspector
            AudioManager.Instance.PlayAudioClip(audioClip);

            // 3. Xóa (hủy) đối tượng đồng xu này khỏi màn hình game sau khi đã ăn xong
            Destroy(this.gameObject);
        }
    }
}