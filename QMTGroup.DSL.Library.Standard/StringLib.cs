namespace QMTGroup.DSL.Library.Standard;


[DSLNamespace("String")]
public class StringLib : IDSLLibrary
{
    public StringLib() { }


    [DSLFunction]
    public string ToUpper(string str) => str.ToUpper();

    [DSLFunction]
    public string ToLower(string str) => str.ToLower();
}
