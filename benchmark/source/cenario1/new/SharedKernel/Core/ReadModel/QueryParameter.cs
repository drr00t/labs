namespace SharedKernel.Core
{
    public class QueryParameter:IParameter
    {
        public QueryParameter(ulong id)
        {
            Id = id;
        }

        public ulong Id { get;}
        
    }
}