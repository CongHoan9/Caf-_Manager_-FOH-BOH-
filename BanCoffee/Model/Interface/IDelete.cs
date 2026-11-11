namespace Model
{
    public interface IDelete<Input, Output>
    {
        Output Delete(Input input);
    }
}
