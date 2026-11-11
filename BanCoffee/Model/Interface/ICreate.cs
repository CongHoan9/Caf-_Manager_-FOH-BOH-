namespace Model
{
    public interface ICreate<Input, Output>
    {
        Output Create(Input input);
    }
}
