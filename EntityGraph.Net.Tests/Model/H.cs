using System.Collections.Generic;

namespace OpenSoftware.EntityGraph.Net.Tests.Model
{
    public class H
    {
        public H()
        {
            Id = IdFactory.Assign;
        }

        public int Id { get; set; }
        public List<Gh> GhSet { get; set; }
        public string Name { get; set; }
    }
}