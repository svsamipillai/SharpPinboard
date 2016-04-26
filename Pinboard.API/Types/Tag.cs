namespace Pinboard.Types
{
    public class Tag
    {
        public string Name { get; set; }

        public int? Count { get; set; }

        public Tag(string name)
        {
            Name = name;
        }

        public Tag()
        {
        }
    }
}