using System;
using System.Collections.Generic;
using System.Linq; // Thư viện hỗ trợ các thao tác xử lý danh sách nhanh (như .ToList())
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    // --- THIẾT LẬP SINGLETON PATTERN ---
    // Giúp các script khác (như script Coin) có thể dễ dàng gọi CoinManager.instance
    public static CoinManager instance;

    // --- CÁC BIẾN QUẢN LÝ ---
    public int points; // Tổng số điểm/số xu mà người chơi đã ăn được hiện tại
    List<Coin> availableCoins; // Danh sách chứa tất cả các đồng xu đang tồn tại trong màn chơi

    void Awake()
    {
        // Khởi tạo Singleton để đảm bảo chỉ có duy nhất 1 bộ quản lý xu trong game
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        points = 0; // Đặt lại điểm số bằng 0 khi vừa bắt đầu vào màn chơi

        // Tự động tìm kiếm TẤT CẢ các đối tượng có gắn script "Coin" đang có trên bản đồ 
        // và chuyển chúng thành một danh sách (List) để tiện quản lý.
        availableCoins = FindObjectsByType<Coin>().ToList();
    }

    /// <summary>
    /// Hàm xử lý khi người chơi ăn được một đồng xu. Được gọi từ script Coin.
    /// </summary>
    /// <param name="coin">Đối tượng đồng xu vừa bị ăn</param>
    /// <param name="coinValue">Giá trị điểm của đồng xu đó</param>
    public void AddPoint(Coin coin, int coinValue)
    {
        points += coinValue; // Cộng thêm điểm vào tổng số điểm hiện tại

        availableCoins.Remove(coin); // Xóa đồng xu vừa ăn ra khỏi danh sách quản lý "các xu còn lại trên bản đồ"

        // Gọi UIManager để cập nhật và hiển thị số điểm mới lên màn hình UI cho người chơi thấy
        UIManager.instance.UpdateCoinText(points);

        // Kiểm tra xem đã ăn hết sạch xu trên bản đồ chưa. 
        // Nếu đã hết xu (IsOutOfCoins trả về true), gọi GameManager để kích hoạt cổng dịch chuyển/cổng về đích.
        if (IsOutOfCoins()) GameManager.instance.SetGateActive(true);
    }

    /// <summary>
    /// Hàm kiểm tra xem trên bản đồ còn đồng xu nào không.
    /// </summary>
    /// <returns>Trả về true nếu đã hết sạch xu, trả về false nếu vẫn còn xu trên bản đồ</returns>
    public bool IsOutOfCoins()
    {
        // Nếu số lượng phần tử trong danh sách bằng 0 nghĩa là đã hết xu
        return availableCoins.Count == 0;
    }
}