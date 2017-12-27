using System.Collections.Generic;

namespace CodeGen.SimplifiedAst
{
    public class Class
    {
        public string Name { get; set; }
        public string Namespace { get; set; }

        public string Fullname => $"{Namespace}.{Name}";
        
        public List<Method> Methods { get; set; }
        public List<Property> Properties { get; set; }
        
        public ITypeReference TypeReference => new ClassTypeReference(this.Name, this.Namespace);


        protected bool Equals(Class other)
        {
            return string.Equals(Name, other.Name) && string.Equals(Namespace, other.Namespace);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Class) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Namespace != null ? Namespace.GetHashCode() : 0);
            }
        }
    }
}