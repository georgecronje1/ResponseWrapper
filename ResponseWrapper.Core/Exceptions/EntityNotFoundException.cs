using System;

namespace ResponseWrapper.Core.Exceptions
{
    /// <summary>
    /// Represents an error when an entity is not found
    /// or does not exist
    /// </summary>
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string entityTypeName) : base($"{entityTypeName} was not found")
        {
        }
        public EntityNotFoundException(string entityTypeName, string entityId) : base($"{entityTypeName} with id:{entityId} was not found")
        {
        }
    }
}
