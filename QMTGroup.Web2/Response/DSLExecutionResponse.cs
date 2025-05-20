using System.Collections;

namespace QMTGroup.Web.Response;

public class DSLExecutionResponse : IEnumerable<DSLExecutionResponseItem>
{
    public List<DSLExecutionResponseItem> _innerItems = new();

    public void Add(DSLExecutionResponseItem item) => _innerItems.Add(item);

    public IEnumerator<DSLExecutionResponseItem> GetEnumerator() => _innerItems.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
