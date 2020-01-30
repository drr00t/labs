using System;

namespace SharedKernel
{
    public interface IDependencyResolver
    {
        TType Resolve<TType>(Type service) where TType:Type;
    }
}