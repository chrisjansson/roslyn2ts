using CodeGen.SimplifiedAst;

namespace CodeGen
{
    public class TaskVisitor : ReplaceTypeVisitorBase
    {
        public override ITypeReference Visit(ClassTypeReference classTypeReference)
        {
            if (classTypeReference.Name == "Task")
            {
                return new PrimitiveTypeReference(PrimitiveKind.Void);
            }

            return classTypeReference;
        }

        public override ITypeReference Visit(GenericClassTypeReference genericClassTypeReference)
        {
            if (genericClassTypeReference.Name == "Task")
            {
                return genericClassTypeReference.InnerType;
            }
            
            return genericClassTypeReference;
        }
    }
}