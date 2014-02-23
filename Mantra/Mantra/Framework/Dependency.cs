using System;

namespace Mantra.Framework
{
    /// <summary>
    /// Indicates that the field is a dependency.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class Dependency : Attribute
    {
        /// <summary>
        /// Gets or sets the name of the group the dependency reference should be retrieved from.
        /// </summary>
        /// <remarks>
        /// Defaults to the local group if not specified.
        /// </remarks>
        public string Group
        {
            get;
            set;
        }
    }
}
