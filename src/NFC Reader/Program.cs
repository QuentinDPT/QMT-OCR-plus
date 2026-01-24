using PCSC.Exceptions;
using PCSC.Utils;
using PCSC;
using PCSC.Monitoring;

static void CheckErr(SCardError err)
{
    if (err != SCardError.Success)
        throw new PCSCException(err,
            SCardHelper.StringifyError(err));
}

try
{
    var e = ContextFactory.Instance.Establish(SCardScope.System);

    var readers = e.GetReaders();

    var effectiveReader = readers.First();
    e.Cancel();
    e.Dispose();

    var monitor = MonitorFactory.Instance.Create(SCardScope.System);

    monitor.CardInserted += (_, _) => Monitor_CardInserted(null, effectiveReader);
    monitor.CardRemoved += (_, _) => Monitor_CardRemoved();

    monitor.Start(effectiveReader);

    while (true) { }
}
catch (PCSCException ex)
{
    Console.WriteLine("Ouch: "
        + ex.Message
        + " (" + ex.SCardError.ToString() + ")");
}

void Monitor_CardRemoved()
{
    Console.WriteLine("Card removed");
}

void Monitor_CardInserted(SCardMonitor eerr, string readerName)
{
     var context = ContextFactory.Instance.Establish(SCardScope.System);

    // Create a reader object using the existing context
    SCardReader reader = new SCardReader(context);

    // Connect to the card
    SCardError err = reader.Connect(readerName,
        SCardShareMode.Shared,
        SCardProtocol.T0 | SCardProtocol.T1);
    CheckErr(err);

    long pioSendPci;
    switch (reader.ActiveProtocol)
    {
        case SCardProtocol.T0:
            pioSendPci = SCardPCI.T0;
            break;
        case SCardProtocol.T1:
            pioSendPci = SCardPCI.T1;
            break;
        default:
            throw new PCSCException(SCardError.ProtocolMismatch,
                "Protocol not supported: "
                + reader.ActiveProtocol.ToString());
    }

    byte[] pbRecvBuffer = new byte[256];

    // Send test command
    byte[] getUID = new byte[] { 0xFF, 0xCA, 0x00, 0x00, 0x00 };
    err = reader.Transmit((nint)pioSendPci, getUID, ref pbRecvBuffer);
    CheckErr(err);

    Console.Write("New card : ");
    for (int i = 0; i < pbRecvBuffer.Length; i++)
        Console.Write("{0:X2} ", pbRecvBuffer[i]);
    Console.WriteLine();

    context.Release();
}