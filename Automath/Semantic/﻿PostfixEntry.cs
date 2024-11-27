using Automath.Elements;

namespace Automath.Semantic
{
    internal class PostfixEntry
    {
        public EEntryType Type { get; set; }


        public int Index { get; set; }

        public PostfixEntry(EEntryType type, int index)
        {
            Type = type;
            Index = index;
        }

        public override string ToString()
        {
            return $"Type: {Type}, Index: {Index}";
        }
    }
}
