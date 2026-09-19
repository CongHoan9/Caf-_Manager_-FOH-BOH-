using Model;
using System.Collections.Generic;

namespace BLL
{
    public partial interface INewsBusiness : ICreate<NewsModel, bool>, IDelete<string, bool>
    {
        List<NewsModel> GetDataAll();
        NewsModel GetDatabyID(string id);
    }
}
