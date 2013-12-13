namespace FluentObjects.Mvc4
{
    internal class ArrayContextElement : IContextElement
    {
        public string Name { get; set; }

        public int Index { get; set; }

        public object Last { get; set; }

        public override string ToString()
        {
            return string.Format("{0}[{1}]", Name, Index);
        }
    }
}
