# roslyn2ts
Codegen Typescript backend API typedefinitions and clients from C# with the help of Roslyn.

# Reasoning
I struggled to find any tools to do this that didn't depend on Visual Studio or reflection. The benefits of not depending on VS are obvious and not depending on reflection means that the source project doesn't have to be a) built and b) be buildable. In practice that means that front-end contracts can be generated wihtout the need to build the back-end at all.

# Limitiations
The code-gen is very limited and very opinionated.

# Todo
- [ ] Upload sample that shows expected output
- [ ] Url generation via attributes
- [ ] dotnet tool


