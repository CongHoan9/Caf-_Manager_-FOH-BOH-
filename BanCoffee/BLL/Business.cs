namespace BLL
{
    public abstract class Business<IRepository>(IRepository res) where IRepository : class
    {
        protected readonly IRepository _res = res;
    }
}
