namespace SharedKernel.Core
{
    public class CommandParameter:IParameter
    {
        public CommandParameter(ulong id)
        {
            Id = id;
        }

        public ulong Id { get;}
    }
}