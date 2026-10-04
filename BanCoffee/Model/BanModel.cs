#pragma warning disable IDE1006
namespace Model
{
    /// <summary>Ban trong quan (bang [ban]) - quan ly cho ngoi.</summary>
    public class BanModel {
        public string ban_id { get; set; }
        public string ten_ban { get; set; }
        public string khu_vuc { get; set; }
        public int so_cho { get; set; }
        public string trang_thai { get; set; }
    }
}


