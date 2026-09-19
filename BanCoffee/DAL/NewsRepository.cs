using DAL.Helper;
using Model;
using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public partial class NewsRepository(IDatabaseHelper dbHelper) : Repository(dbHelper), INewsRepository
    {
        public bool Create(NewsModel model) => ExecuteTransaction("sp_news_create",
                                                                  "@news_id", model.news_id,
                                                                  "@title", model.title,
                                                                  "@content_news", model.content_news);
        public bool Delete(string id) => ExecuteTransaction("sp_news_delete", "@news_id", id);
        public List<NewsModel> GetDataAll() => ExecuteQuery<NewsModel>("sp_news_all");
        public NewsModel GetDatabyID(string id) => ExecuteQuery<NewsModel>("sp_news_get_by_id", "@news_id", id).FirstOrDefault();
    }
}
