namespace SharedKernel.Core
{
    public interface IRequestValidation<TData>
    {
        string Name { get; }
        ValidationResult Validate(TData data);
    }
}