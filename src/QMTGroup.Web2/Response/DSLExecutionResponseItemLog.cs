namespace QMTGroup.Web.Response;

public class DSLExecutionResponseItemLog : DSLExecutionResponseItem
{
    public DSLExecutionResponseItemLog() => State = DSLExecutionResponseState.Running;

    public string LoggedLine { get; set; }
}
