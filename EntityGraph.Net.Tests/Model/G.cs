using System.Collections.Generic;

namespace OpenSoftware.EntityGraph.Net.Tests.Model
{
    public class G
    {
        public G()
        {
            Id = IdFactory.Assign;
        }

        public int Id { get; set; }
        public List<Gh> GhSet { get; set; } = new List<Gh>();
        public GEnum GEnum { get; set; }
    }
}