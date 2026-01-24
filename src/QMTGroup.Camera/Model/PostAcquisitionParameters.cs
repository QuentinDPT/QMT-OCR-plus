namespace QMTGroup.Camera;

public record PostAcquisitionParameters
{
    public QuarterRotation Rotation { get; set; } = QuarterRotation.Deg0;

    public bool VerticalFlip { get; set; } = false;

    public bool HorizontalFlip { get; set; } = false;

    public bool ForceGrayScale { get; set; } = false;
}
