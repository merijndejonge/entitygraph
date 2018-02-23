using OpenSoftware.EntityGraphCore;

namespace OpenSoftware.EntityGraph.Net
{
    /// <inheritdoc />
    /// <summary>
    /// This class defines an entity graph shape that spans all associations in your object
    /// graph that are annotated with the AssociationAttribute.
    /// </summary>
    public class FullEntityGraphShape : DynamicGraphShape
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the FullEntityGraphShape class
        /// </summary>
        public FullEntityGraphShape() :
            base(x => Helpers.IsAssociation(x.PropertyType))
        {
        }
    }
}
