namespace SharedKernel.Core
{
    public interface IOperation
    {
        void Post<TParameter>(TParameter parameter);
    }
}