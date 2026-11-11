namespace Model
{
    public interface IUpdate<Input, Output>
    {
        Output Update(Input input);
    }
}
