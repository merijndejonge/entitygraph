using System.Collections.Generic;

namespace OpenSoftware.EntityGraph.Net.Tests.Model
{
    public class F
    {
        public F()
        {
            Id = IdFactory.Assign;
        }

        public int Id { get; set; }
        public List<E> ESet { get; set; } = new List<E>();
    }
}