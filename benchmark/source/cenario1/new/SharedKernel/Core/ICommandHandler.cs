using System.Threading.Tasks;

namespace SharedKernel.Core
{
    public interface ICommandHandler<TCommand>
    {
        Task Execute(TCommand parameter);
    }
}