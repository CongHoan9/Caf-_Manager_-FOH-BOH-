using Model;
using Microsoft.Extensions.Configuration;

namespace BLL
{
    public partial class NewsBusiness(ICreate<NewsModel, bool> res, IConfiguration configuration) : Business<ICreate<NewsModel, bool>>(res), INewsBusiness
    {
        private readonly string Secret = configuration["AppSettings:Secret"];
        public bool Create(NewsModel model)
        {
            return _res.Create(model);
        }
    }

}
