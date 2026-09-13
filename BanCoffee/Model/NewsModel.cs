using System;

namespace Model
{
    /// <summary>Tin tuc - khuyen mai (bang [news]); created_date do SP tu gan GETDATE().</summary>
    public class NewsModel
    {
        public string News_id { get; set; }
        public string Title { get; set; }
        public DateTime Created_date { get; set; }
        public string Content_news { get; set; }
    }
}
