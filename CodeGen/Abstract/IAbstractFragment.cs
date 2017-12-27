using System.Collections.Generic;
using CodeGen.SimplifiedAst;

namespace CodeGen.Abstract
{
    public interface IAbstractFragment
    {
        IReadOnlyCollection<ITypeReference> GetDependencies();
        IReadOnlyCollection<ITypeReference> GetExports();
    }
}