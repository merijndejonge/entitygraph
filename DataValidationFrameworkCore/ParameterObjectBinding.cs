using System;

namespace OpenSoftware.DataValidationFrameworkCore
{
    /// <inheritdoc />
    /// <summary>
    /// Class that represents a binding between a dependency parameter and an object.
    /// For a validation rule dependency
    ///    A =&gt; A.B.c (where type of A equals A)
    /// And an object
    ///    var a = new A { B = new B() };
    /// ParameterObject equals a.
    /// ParameterObjectType equals A (i.e., the type of parameter A)
    /// ParameterName equals 'A'
    /// </summary>
    internal class ParameterObjectBinding : IEquatable<ParameterObjectBinding>
    {
        /// <summary>
        /// Represents the object that is bound to the parameter 'A' of a valudation rule dependency 'A => A.some.path.p'.
        /// </summary>
        public object ParameterObject { get; internal set; }

        /// <summary>
        /// Represents the type of the parameter 'A' of a valudation rule dependency 'A => A.some.path.p'.
        /// </summary>
        public Type ParameterObjectType { get; internal set; }

        /// <summary>
        /// Represents the name of the parameter 'A' of a valudation rule dependency 'A => A.some.path.p'.
        /// </summary>
        public string ParameterName { get; internal set; }

        /// <inheritdoc />
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ParameterObjectBinding other)
        {
            if (other is null) return false;

            //Check whether the compared object references the same data.
            if (ReferenceEquals(this, other)) return true;

            //Check whether the products' properties are equal.
            return ParameterObject == other.ParameterObject
                   && ParameterObjectType == other.ParameterObjectType
                   && ParameterName == other.ParameterName;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int hashParameterObject = ParameterObject == null ? 0 : ParameterObject.GetHashCode();
            int hashParameterObjectType = ParameterObjectType == null ? 0 : ParameterObjectType.GetHashCode();
            int hashParameterName = ParameterName == null ? 0 : ParameterName.GetHashCode();

            return hashParameterObject ^ hashParameterObjectType ^ hashParameterName;
        }
    }
}