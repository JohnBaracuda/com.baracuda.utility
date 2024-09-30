namespace Baracuda.Utility.Editor.Drawer
{
    public readonly struct FoldoutData
    {
        public readonly string Title;
        public readonly string Tooltip;

        public FoldoutData(string title, string tooltip)
        {
            Title = title;
            Tooltip = tooltip;
        }

        public static (string, string) Deconstruct(FoldoutData data)
        {
            return (data.Title, data.Tooltip);
        }

        public void Deconstruct(out string title, out string tooltip)
        {
            title = Title;
            tooltip = Tooltip;
        }

        public static implicit operator FoldoutData(string title)
        {
            return new FoldoutData(title, string.Empty);
        }

        public override bool Equals(object obj)
        {
            return obj is FoldoutData data && Equals(data);
        }

        public bool Equals(FoldoutData other)
        {
            return Title == other.Title;
        }

        public static bool operator ==(FoldoutData rhs, FoldoutData lhs)
        {
            return rhs.Title == lhs.Title;
        }

        public static bool operator !=(FoldoutData rhs, FoldoutData lhs)
        {
            return !(rhs == lhs);
        }

        public override int GetHashCode()
        {
            return Title.GetHashCode();
        }
    }
}