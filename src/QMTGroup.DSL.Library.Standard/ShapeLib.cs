namespace QMTGroup.DSL.Library.Standard;

[DSLNamespace("Shape")]
public class ShapeLib : IDSLLibrary
{
    public ShapeLib() { }


    [DSLFunction]
    public object NewCircle()
    {
        return "bonjour";
    }
}
