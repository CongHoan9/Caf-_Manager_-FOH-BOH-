namespace Model
{
    /// <summary>Hop dong cap nhat.</summary>
    public interface IUpdate<Input, Output>
    {
        Output Update(Input input);
    }
}
