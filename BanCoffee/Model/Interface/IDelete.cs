namespace Model
{
    /// <summary>Hop dong xoa theo khoa.</summary>
    public interface IDelete<Input, Output>
    {
        Output Delete(Input input);
    }
}
