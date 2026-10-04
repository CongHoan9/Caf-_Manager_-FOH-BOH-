namespace Model
{
    /// <summary>Hop dong tao moi: Repository va Business ke thua truc tiep.</summary>
    public interface ICreate<Input, Output>
    {
        Output Create(Input input);
    }
}
