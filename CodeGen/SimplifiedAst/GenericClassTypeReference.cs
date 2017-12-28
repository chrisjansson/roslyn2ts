namespace CodeGen.SimplifiedAst
{
    public class GenericClassTypeReference : ITypeReference
    {
        public GenericClassTypeReference(string name, string @namespace, ITypeReference innerType)
        {
            Name = name;
            Namespace = @namespace;
            InnerType = innerType;
        }

        public string Name { get; }
        public string Namespace { get; }
        public ITypeReference InnerType { get; }

        public T Visit<T>(ITypeReferenceVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}