namespace BLL
{
    /// <summary>Lop nen cho cac Business: giu repository tu DI.</summary>
    public abstract class Business<IRepository>(IRepository res) where IRepository : class
    {
        protected readonly IRepository _res = res;
    }
}
