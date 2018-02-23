namespace OpenSoftware.EntityGraph.Net.Tests
{
    public static class IdFactory
    {
        private static int Id { get; set; }

        public static int Assign
        {
            get
            {
                if (AutoGenerateKeys)
                {
                    Id--;
                    return Id;
                }

                return 0;
            }
        }

        public static bool AutoGenerateKeys { get; set; }
    }
}