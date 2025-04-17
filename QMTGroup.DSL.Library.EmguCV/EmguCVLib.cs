using Emgu.CV;
using QMTGroup.DSL.Hub;
using QMTGroup.Image;

namespace QMTGroup.DSL.Library.EmguCV;

[DSLNamespace("EmguCV")]
public class EmguCVLib : IDSLLibrary
{
    private IResourceHub _resourceHub;

    public EmguCVLib(IResourceHub hub)
    {
        _resourceHub = hub;
    }

    [DSLFunction]
    public Guid ToGrayScales(string imageInput, string imageOutput = "")
    {
        var imageInputGuid = Guid.Parse(imageInput);

        Matrix img = _resourceHub.Get<Matrix>(imageInputGuid) ?? throw new ResourceNotFoundException();

        Guid output = Guid.NewGuid();

        var gray = _toGrayScales(img);

        _resourceHub.Set(output, gray);

        return output;
    }

    private Matrix _toGrayScales(Matrix input)
    {
        var mat = input.ToMat();

        Mat result = new Mat();

        CvInvoke.CvtColor(mat, result, Emgu.CV.CvEnum.ColorConversion.Gray2Bgr);

        return result.ToMatrix();
    }
}
