#pragma warning disable IDE1006
using System;

namespace Model
{
    /// <summary>Tin tuc - khuyen mai (bang [news]); created_date do SP tu gan GETDATE().</summary>
    public class NewsModel {
        public string news_id { get; set; }
        public string title { get; set; }
        public DateTime created_date { get; set; }
        public string content_news { get; set; }
    }
}


