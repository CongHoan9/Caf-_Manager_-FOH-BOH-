using DAL.Helper;
using Model;
namespace DAL
{
    public partial class NewsRepository(IDatabaseHelper dbHelper) : Repository(dbHelper), INewsRepository
    {
        public bool Create(NewsModel model) => Execute(() => ExecuteTransaction("sp_news_create",
                                                                                "@news_id", model.news_id,
                                                                                "@title", model.title,
                                                                                "@content_news", model.content_news));
    }
}
