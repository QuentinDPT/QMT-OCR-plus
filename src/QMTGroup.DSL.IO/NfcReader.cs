using QMTGroup.Core;
using QMTGroup.DSL.Library;

namespace QMTGroup.DSL.IO;

[DSLNamespace("NFC")]
public class NfcReader : IDSLLibrary
{
    private readonly IWebLogger _webLogger;

    private Action<string>? _action = null;

    public NfcReader(IWebLogger webLogger)
    {
        _webLogger = webLogger;
    }

    [DSLFunction]
    public void CreateReader()
    {
        _action = null;
    }

    [DSLFunction]
    public void OnRead(Action<string> action)
    {
        _action = action;
    }
}
