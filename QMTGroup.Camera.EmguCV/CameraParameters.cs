using Emgu.CV;
using Emgu.CV.CvEnum;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace QMTGroup.Camera.EmguCV;

public class CameraParameters
{
    public int Slot { get; set; } = 0;

    public bool FlipVertical { get; set; } = false;

    public bool FlipHorizontal { get; set; } = false;

    public Rotation Rotation { get; set; } = Rotation.Rotate0;

    public Dictionary<Urn.Urn, double> UserParamters { get; set; } = new();

    public IEnumerable<string> AvailableParameters => InternalDefaultParameters;

    [JsonIgnore]
    internal List<string> InternalDefaultParameters = new();
}
