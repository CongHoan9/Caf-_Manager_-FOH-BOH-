using DAL;
using Model;
using System;
using System.Collections.Generic;

namespace BLL
{
    public partial class NewsBusiness(INewsRepository res) : Business<INewsRepository>(res), INewsBusiness
    {
        public bool Create(NewsModel model)
        {
            model.news_id ??= Guid.NewGuid().ToString();
            return _res.Create(model);
        }

        public bool Delete(string id)
        {
            return _res.Delete(id);
        }

        public List<NewsModel> GetDataAll()
        {
            return _res.GetDataAll();
        }

        public NewsModel GetDatabyID(string id)
        {
            return _res.GetDatabyID(id);
        }
    }
}
