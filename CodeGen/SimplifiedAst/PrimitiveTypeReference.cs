namespace CodeGen.SimplifiedAst
{
    public class PrimitiveTypeReference : ITypeReference

    {
        public PrimitiveTypeReference(PrimitiveKind kind)

        {
            Kind = kind;
        }


        public PrimitiveKind Kind { get; }
        
        public T Visit<T>(ITypeReferenceVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class ClassTypeReference : ITypeReference
    {
        public string Name { get; }

        public ClassTypeReference(string name, string @namespace)
        {
            Name = name;
        }

        protected bool Equals(ClassTypeReference other)
        {
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ClassTypeReference) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public T Visit<T>(ITypeReferenceVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}