namespace FluentObjects.Mvc4
{
    internal class ObjectContextElement : IContextElement
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
