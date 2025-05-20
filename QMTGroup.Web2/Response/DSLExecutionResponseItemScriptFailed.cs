namespace QMTGroup.Web.Response;

public class DSLExecutionResponseItemScriptFailed : DSLExecutionResponseItem
{
    public DSLExecutionResponseItemScriptFailed() => State = DSLExecutionResponseState.Failed;
}
