using System;
using CodeGen.SimplifiedAst;

namespace CodeGen
{
    public class TypescriptTypeReferenceTranslator : ITypeReferenceVisitor<string>
    {
        public string Visit(PrimitiveTypeReference primitiveTypeReference)
        {
            switch (primitiveTypeReference.Kind)
            {
                case PrimitiveKind.Double:
                case PrimitiveKind.Decimal:
                case PrimitiveKind.Int32:
                    return "number";
                case PrimitiveKind.String:
                    return "string";
                case PrimitiveKind.Void:
                    return "void";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public string Visit(CollectionTypeReference collectionTypeReference)
        {
            return collectionTypeReference.InnerType.Visit(this) + "[]";
        }

        public string Visit(ClassTypeReference classTypeReference)
        {
            return classTypeReference.Name;
        }

        public string Visit(GenericClassTypeReference genericClassTypeReference)
        {
            throw new NotImplementedException();
        }

        public string Accept(ITypeReference type)
        {
            return type.Visit(this);
        }
    }
}