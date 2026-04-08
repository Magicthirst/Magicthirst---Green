using System;

namespace VContainer.Internal
{
    public interface IClosedRegistrationProvider
    {
        Registration GetClosedRegistration(Type closedInterfaceType, Type[] typeParameters);
    }
}