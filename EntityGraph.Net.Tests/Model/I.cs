namespace OpenSoftware.EntityGraph.Net.Tests.Model
{
    public class I
    {
        public I()
        {
            Id = IdFactory.Assign;
        }

        public int Id { get; set; }

        public double[] X { get; set; }

        public string AString { get; set; }
    }
}