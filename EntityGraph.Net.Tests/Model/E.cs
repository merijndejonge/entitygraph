namespace OpenSoftware.EntityGraph.Net.Tests.Model
{
    public class E
    {
        public E()
        {
            Id = IdFactory.Assign;
        }

        public int Id { get; set; }

        public F F { get; set; }
        public int? FId { get; set; }
    }
}