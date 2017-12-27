using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGen.SimplifiedAst
{
    public class ClassCollector : CSharpSyntaxWalker
    {
        private readonly SemanticModel _document;
        public readonly List<Class> Result = new List<Class>();

        public ClassCollector(SemanticModel document)
        {
            _document = document;
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var namedTypeSymbol = _document.GetDeclaredSymbol(node);

            if (!node.Modifiers.Any(x => x.IsKind(SyntaxKind.PublicKeyword)))
                return;

            var collector = new MemberSymbolCollector();

            foreach (var member in namedTypeSymbol.GetMembers())
                collector.Visit(member);


            Result.Add(new Class
            {
                Name = namedTypeSymbol.Name,
                Namespace = namedTypeSymbol.ContainingNamespace.Name,
                Methods = collector.Result,
                Properties = collector.Properties
            });
        }

        public class EnumerableCollector : SymbolVisitor<(bool, ITypeSymbol)>
        {
            public override (bool, ITypeSymbol) VisitArrayType(IArrayTypeSymbol symbol)
            {
                if (symbol.Rank != 1)
                    return (false, null);

                return (true, symbol.ElementType);
            }

            public override (bool, ITypeSymbol) VisitNamedType(INamedTypeSymbol symbol)
            {
                if (IsEnumerable(symbol))
                {
                    var typeArgument1 = symbol.TypeArguments.Single();
                    return (true, typeArgument1);
                }

                var iface = symbol.AllInterfaces
                    .SingleOrDefault(IsEnumerable);
                if (iface == null)
                    return (false, null);


                var typeArgument = symbol.TypeArguments.Single();
                return (true, typeArgument);
            }

            private static bool IsEnumerable(INamedTypeSymbol symbol)
            {
                if (symbol.IsGenericType)
                {
                    var constructUnboundGenericType = symbol.ConstructUnboundGenericType();
                    if (constructUnboundGenericType.Name == "IEnumerable")
                        return true;
                }

                return false;
            }
        }

        public class MemberSymbolCollector : SymbolVisitor
        {
            public readonly List<Property> Properties = new List<Property>();
            public readonly List<Method> Result = new List<Method>();

            public override void VisitMethod(IMethodSymbol symbol)
            {
                if (symbol.DeclaredAccessibility != Accessibility.Public)
                    return;

                var parameters = symbol.Parameters
                    .Select(x => new Parameter
                    {
                        Name = x.Name,
                        Type = ToReturnType(x.Type)
                    })
                    .ToList();

                Result.Add(new Method
                {
                    Name = symbol.Name,
                    Parameters = parameters,
                    ReturnType = ToReturnType(symbol.ReturnType)
                });
            }

            public override void VisitProperty(IPropertySymbol symbol)
            {
                if (symbol.DeclaredAccessibility != Accessibility.Public)
                    return;

                Properties.Add(new Property
                {
                    Name = symbol.Name,
                    Type = ToReturnType(symbol.Type)
                });
            }

            private ITypeReference ToReturnType(ITypeSymbol type)
            {
                if (type.Name == "Void")
                    return new PrimitiveTypeReference(PrimitiveKind.Void);
                if (type.Name == "Int32")
                    return new PrimitiveTypeReference(PrimitiveKind.Int32);
                if (type.Name == "String")
                    return new PrimitiveTypeReference(PrimitiveKind.String);
                if (type.Name == "Double")
                    return new PrimitiveTypeReference(PrimitiveKind.Double);
                if (type.Name == "Decimal")
                    return new PrimitiveTypeReference(PrimitiveKind.Decimal);

                var abc = new EnumerableCollector();
                var (isIEnumerable, innerType) = abc.Visit(type);

                if (isIEnumerable)
                    return new CollectionTypeReference(ToReturnType(innerType));

                if (type.IsReferenceType)
                {
                    return new ClassTypeReference(type.Name, type.ContainingNamespace.Name);
                }

                throw new NotSupportedException(type.Name);
            }
        }
    }
}