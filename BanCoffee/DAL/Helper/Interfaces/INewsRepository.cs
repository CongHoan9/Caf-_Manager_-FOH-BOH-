using Model;
using System.Collections.Generic;
namespace DAL
{
    public interface INewsRepository : ICreate<NewsModel, bool>, IDelete<string, bool>
    {
        List<NewsModel> GetDataAll();
        NewsModel GetDatabyID(string id);
    }
}
