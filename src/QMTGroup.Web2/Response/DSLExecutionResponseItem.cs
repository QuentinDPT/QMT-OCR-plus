namespace QMTGroup.Web.Response;

public class DSLExecutionResponseItem
{
    public DSLExecutionResponseState State { get; protected set; }

    public DateTime TimeStamp { get; private set; } = DateTime.Now;
}