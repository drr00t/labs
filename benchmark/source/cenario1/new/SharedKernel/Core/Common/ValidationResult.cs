using System.Collections;
using System.Collections.Generic;

namespace SharedKernel.Core
{
    public class ValidationItem
    {
        public string Message { get; }
    }
    
    public class ValidationResult: IEnumerable<ValidationItem>
    {
        private readonly IList<ValidationItem> _validations;

        public ValidationResult():this(new List<ValidationItem>())
        {
        }
        
        public ValidationResult(IList<ValidationItem> validations)
        {
            _validations = validations;
        }
        
            
        public IEnumerator<ValidationItem> GetEnumerator()
        {
            return _validations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}