namespace Model
{
    /// <summary>Response chuan cho cac endpoint search (phan trang).</summary>
    public class ResponseModel
    {
        public long TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public dynamic Data { get; set; }
    }
}
