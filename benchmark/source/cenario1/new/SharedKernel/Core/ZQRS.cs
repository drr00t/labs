using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;

namespace SharedKernel.Core
{
    // tornar isso assíncrono

    #region command side

    #endregion
    
    #region query side

    #endregion
    
    #region persistence
    
    /*
     * message  fields
     *     route key
     *     operation string
     *     empty
     *     fragmented indicator boolean
     *     fragment number
     *     fraglimit
     *     data blob
     * 
     *     states
     *         hello
     *         ready
     *         processing
     *         failed
     *     
     */

    public interface IEventSubmitter
    {
        Task Submit<TData>(TData data);
    }

    public class EventSubmitter : IEventSubmitter
    {
        private readonly String[] _endPOints; 
        
        public EventSubmitter(String[] endPoints)
        {
            _endPOints = endPoints;
        }

        public async Task Submit<TData>(TData data)
        {
            throw new NotImplementedException();
        }

        private void Connect()
        {
           
        }
    }
    
    
   #region write model
    
   public interface IEventStore
   {
       void AppendEvent();
       void TakeSnapshot();
       void Replicate();
   }

   public interface IEventStoreExecutor
   {
       void Store();
   }
    
    #endregion
    
    #region read model
    
    
    #endregion
    
    #endregion
}