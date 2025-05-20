namespace QMTGroup.Web.Response;

public class DSLExecutionResponseItemScriptSuccess : DSLExecutionResponseItem
{
    public DSLExecutionResponseItemScriptSuccess() => State = DSLExecutionResponseState.Failed;
}
