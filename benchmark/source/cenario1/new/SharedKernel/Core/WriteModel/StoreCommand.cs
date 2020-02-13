namespace SharedKernel.Core
{
    public class StoreCommand:IParameter
    {
        public StoreCommand(ulong id, string commandType, byte[] commandData)
        {
            Id = id;
            CommandType = commandType;
            CommandData = commandData;
        }

        public ulong Id { get;}
        public string CommandType { get;}
        public byte[] CommandData { get; }
        
    }
}