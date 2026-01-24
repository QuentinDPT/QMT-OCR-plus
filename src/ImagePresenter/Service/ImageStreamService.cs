namespace ImagePresenter;

public class ImageStreamService
{
    private static AcquisitionProcessService? _acq = null;

    public ImageStreamService()
    {
        if (_acq == null)
        {
            _acq = new AcquisitionProcessService()
            {
                ProcessLocation = @"D:\Dev\PERSO\QMT-OCR-plus\ImageProducer\bin\Debug\net8.0\ImageProducer.exe",
            };
            _acq.OnProcessExit += _acq_OnProcessExit;
        }
    }

    private void _acq_OnProcessExit(object? sender, EventArgs e)
    {
        if (_acq is null)
            return;

        _acq.Stop();
        _acq.Start();
    }

    public void Start() => _acq?.Start();

    public void Stop() => _acq?.Stop();

    public string Read() => _acq?.ReadString() ?? string.Empty;
}
