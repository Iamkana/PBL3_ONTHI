namespace PBL3.Shared.DTOs.Banners // Định nghĩa namespace PBL3.Shared.DTOs.Banners quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// Request cập nhật banner. Schema giống Create — kế thừa để chia sẻ validator.
    /// </summary>
    public class UpdateBannerRequest : CreateBannerRequest // Định nghĩa lớp DTO truyền tải dữ liệu UpdateBannerRequest triển khai/kế thừa CreateBannerRequest.
    {
    }
}
