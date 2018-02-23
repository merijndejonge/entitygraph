using System;

namespace OpenSoftware.EntityGraphCore
{
    /// <inheritdoc />
    /// <summary>
    /// Specifies that an edge is part of a graph of related entities 
    /// that should be handled as a unit during cloning, detaching, deleting, and so on.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class EntityGraphAttribute : Attribute
    {
        /// <summary>
        /// Gets/sets the name of the entity graph
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Constructor. Sets the name of the entity graph to 'name'
        /// </summary>
        /// <param name="name"></param>
        public EntityGraphAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public EntityGraphAttribute()
        {
            Name = null;
        }
    }
}