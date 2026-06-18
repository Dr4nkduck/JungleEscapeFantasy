using UnityEngine;
using TMPro; // Khai báo thư viện để sử dụng TextMeshPro (hiển thị chữ sắc nét trong Unity)

public class UIManager : MonoBehaviour
{
    // --- CÁC BIẾN CẤU HÌNH (Giao diện hiển thị) ---
    [Header("UI Canvases")]
    public GameObject LoseGameCanvas; // Giao diện màn hình khi Thua (Lose)
    public GameObject WinGameCanvas;  // Giao diện màn hình khi Thắng (Win)

    [Header("UI Texts")]
    public TextMeshProUGUI[] coinTexts; // Mảng chứa các đối tượng Text hiển thị số điểm/số xu thu thập được

    // --- THIẾT LẬP SINGLETON PATTERN ---
    // Biến static giúp các script khác dễ dàng gọi hàm từ UIManager mà không cần kéo thả reference
    public static UIManager instance;

    void Awake()
    {
        // Khởi tạo Singleton: Đảm bảo chỉ có duy nhất một UIManager tồn tại trong Scene
        if (instance == null)
        {
            instance = this; // Gán instance bằng chính script này
        }
    }

    // --- CÁC HÀM CHỨC NĂNG CHÍNH ---

    /// <summary>
    /// Cập nhật số lượng xu lên toàn bộ các text hiển thị điểm trên màn hình.
    /// </summary>
    /// <param name="points">Số điểm/Số xu hiện tại</param>
    public void UpdateCoinText(int points)
    {
        // Duyệt qua từng ô Text có trong mảng coinTexts và cập nhật lại chuỗi ký tự bằng số điểm mới
        foreach (var i in coinTexts)
        {
            i.text = points.ToString(); // Chuyển kiểu số (int) thành kiểu chữ (string) để hiển thị
        }
    }

    /// <summary>
    /// Bật hoặc tắt màn hình Thua Game.
    /// </summary>
    /// <param name="enabled">true để bật, false để tắt</param>
    public void ToggleLoseGameCanvas(bool enabled)
    {
        LoseGameCanvas.SetActive(enabled); // Kích hoạt hoặc hủy kích hoạt GameObject LoseGameCanvas
    }

    /// <summary>
    /// Bật hoặc tắt màn hình Thắng Game.
    /// </summary>
    /// <param name="enabled">true để bật, false để tắt</param>
    public void ToggleWinGameCanvas(bool enabled)
    {
        WinGameCanvas.SetActive(enabled); // Kích hoạt hoặc hủy kích hoạt GameObject WinGameCanvas
    }
}