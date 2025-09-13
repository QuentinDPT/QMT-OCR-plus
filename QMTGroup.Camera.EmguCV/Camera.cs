using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using QMTGroup.Image;
using QMTGroup.Urn;
using System.Drawing;

namespace QMTGroup.Camera.EmguCV;

public class Camera : ICamera
{
    private Mat _imageRecived;

    private VideoCapture? _videoCapture = null;

    public event EventHandler<Matrix> OnReciveImage;

    private CameraParameters _cameraParameters;

    public CameraParameters Parameters => _cameraParameters;

    private CameraStatus _cameraStatus = CameraStatus.Stopped;

    private Action<Mat> _applyRotation;

    public CameraStatus Status => _cameraStatus;

    public Camera(CameraParameters cameraParameters)
    {
        _cameraParameters = cameraParameters;
    }

    public void StartCapture()
    {
        if (_videoCapture is not null)
            return;

        _imageRecived = new Mat();

        Tuple<CapProp, int>[] userProperties = _cameraParameters.UserParamters
            .Select(x => System.Tuple.Create((CapProp)Enum.Parse(typeof(CapProp), x.Key.Last(), true), (int)x.Value))
            .ToArray();

        _videoCapture = new VideoCapture(_cameraParameters.Slot, VideoCapture.API.Any, userProperties);

        _cameraParameters.InternalDefaultParameters.Clear();
        _cameraParameters.InternalDefaultParameters = Enum.GetValues<CapProp>().Select(x => Enum.GetName(x) ?? string.Empty).Distinct().Order().ToList();

        _videoCapture.FlipVertical = _cameraParameters.FlipVertical;
        _videoCapture.FlipHorizontal = _cameraParameters.FlipHorizontal;

        switch (_cameraParameters.Rotation)
        {
            case Rotation.Rotate90:
                _applyRotation = x => {
                    CvInvoke.Rotate(x, x, RotateFlags.Rotate90Clockwise);
                };
                break;
            case Rotation.Rotate180:
                _applyRotation = x => {
                    CvInvoke.Rotate(x, x, RotateFlags.Rotate180);
                };
                break;
            case Rotation.Rotate270:
                _applyRotation = x => {
                    CvInvoke.Rotate(x, x, RotateFlags.Rotate90CounterClockwise);
                };
                break;
            default:
                _applyRotation = x => { };
                break;
        }

        foreach (var param in _cameraParameters.UserParamters)
        {
            string paramName = param.Key.ToString().Split(":").Last();

            if (!Enum.TryParse(paramName, true, out CapProp paramEnum))
                continue;

            _videoCapture.Set(paramEnum, param.Value);
        }

        _videoCapture.ImageGrabbed += (object sender, EventArgs e) =>
        {
            try
            {
                _videoCapture?.Retrieve(_imageRecived);
                _applyRotation(_imageRecived);
                _monTest(_imageRecived);
                OnReciveImage?.Invoke(this, _imageRecived.ToMatrix());
            }
            catch (Exception)
            {
                _internalDispose();
            }
        };

        _videoCapture.Start();

        _cameraStatus = CameraStatus.Started;
    }

    private void _monTest(Mat imageRecived)
    {

        using var gray = new Mat();
        using var blur = new Mat();
        using var edges = new Mat();
        using var edges2 = new Mat();

        CvInvoke.CvtColor(imageRecived, gray, ColorConversion.Bgr2Gray);
        CvInvoke.GaussianBlur(gray, blur, new Size(5, 5), 1.2);
        CvInvoke.Canny(blur, edges, 50, 150);
        CvInvoke.Dilate(edges, edges2, null, new Point(-1, -1), 1, BorderType.Reflect, default);

        // 1) Détecter tous les rectangles
        var circles = FindCircles(imageRecived);
        var rects = FindRectangles(circles, minArea: 800, angleToleranceDeg: 15, epsilonRatio: 0.02);

        /*
        switch (DateTime.Now.Ticks/3000000 % 5)
        {
            case 1:
                CvInvoke.CvtColor(gray, _imageRecived, ColorConversion.Gray2Bgr);
                break;
            case 2:
                CvInvoke.CvtColor(blur, _imageRecived, ColorConversion.Gray2Bgr);
                break;
            case 3:
                CvInvoke.CvtColor(edges, _imageRecived, ColorConversion.Gray2Bgr);
                break;
            case 4:
                CvInvoke.CvtColor(edges2, _imageRecived, ColorConversion.Gray2Bgr);
                break;
            default:
                break;
        }
        //*/
        //CvInvoke.CvtColor(edges2, _imageRecived, ColorConversion.Gray2Bgr);

        // 2) Dessiner tous les rectangles détectés (verts)
        foreach (var quad in rects.Item1)
            CvInvoke.Polylines(imageRecived, quad, true, new MCvScalar(0, 255, 0), 2);
        foreach (var quad in rects.Item2)
            CvInvoke.Polylines(imageRecived, quad, true, new MCvScalar(255, 0, 0), 2);
    }

    #region bazar

    static Mat FindCircles(Mat bgr)
    {
        // Convertir en niveaux de gris
        Mat gray = new Mat();
        CvInvoke.CvtColor(bgr, gray, ColorConversion.Bgr2Gray);

        // Flouter pour réduire le bruit
        CvInvoke.GaussianBlur(gray, gray, new Size(9, 9), 2, 2);

        // Détection des cercles via HoughCircles
        CircleF[] cercles = CvInvoke.HoughCircles(
            gray,
            HoughModes.Gradient,  // Méthode standard de Hough
            1.0,                  // Résolution de l’accumulateur (1 = même taille que l'image)
            5.0,                  // Distance minimale entre centres de cercles
            100.0,                // Seuil plus élevé pour Canny
            50.0,                 // Seuil accumulateur (plus petit = plus de détections)
            10,                   // Rayon minimum
            200                   // Rayon maximum
        );

        // Dessiner les cercles détectés
        foreach (CircleF c in cercles)
        {
            CvInvoke.Circle(bgr, Point.Round(c.Center), (int)c.Radius, new MCvScalar(255, 255, 0), 2);
            CvInvoke.Circle(bgr, Point.Round(c.Center), 2, new MCvScalar(255, 0, 255), 3); // centre
        }

        return bgr;
    }

    // Détecte des quadrilatères rectangulaires (angles ~ 90°) et convexes
    static (List<Point[]>, List<Point[]>) FindRectangles(Mat bgr,
                                        double minArea = 1200,
                                        double angleToleranceDeg = 15.0,
                                        double epsilonRatio = 0.02)
    {
        using var gray = new Mat();
        using var blur = new Mat();
        using var edges = new Mat();

        CvInvoke.CvtColor(bgr, gray, ColorConversion.Bgr2Gray);
        CvInvoke.GaussianBlur(gray, blur, new Size(5, 5), 1.2);
        CvInvoke.Canny(blur, edges, 50, 150);
        CvInvoke.Dilate(edges, edges, null, new Point(-1, -1), 1, BorderType.Reflect, default);

        var contours = new VectorOfVectorOfPoint();
        CvInvoke.FindContours(edges, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

        var result = new List<Point[]>();
        var rejected = new List<Point[]>();

        for (int i = 0; i < contours.Size; i++)
        {
            using var c = contours[i];
            double area = CvInvoke.ContourArea(c);
            if (area < minArea) continue;

            double peri = CvInvoke.ArcLength(c, true);
            using var approx = new VectorOfPoint();
            CvInvoke.ApproxPolyDP(c, approx, epsilonRatio * peri, true);
            if (approx.Size != 4) continue;

            var quad = approx.ToArray();
            if (!CvInvoke.IsContourConvex(approx)) continue;

            // Vérif ~ angles droits (tolérance)
            var ordered = OrderCorners(quad);
            double a0 = AngleDeg(ordered[3], ordered[0], ordered[1]);
            double a1 = AngleDeg(ordered[0], ordered[1], ordered[2]);
            double a2 = AngleDeg(ordered[1], ordered[2], ordered[3]);
            double a3 = AngleDeg(ordered[2], ordered[3], ordered[0]);

            if (IsNearRightAngle(a0, angleToleranceDeg) &&
                IsNearRightAngle(a1, angleToleranceDeg) &&
                IsNearRightAngle(a2, angleToleranceDeg) &&
                IsNearRightAngle(a3, angleToleranceDeg))
            {
                result.Add(ordered);
            }
            else
            {
                rejected.Add(ordered);
            }
        }

        // Tri par aire décroissante (optionnel)
        result = result.OrderByDescending(q => QuadArea(q)).ToList();
        return (result, rejected);
    }

    static bool IsNearRightAngle(double deg, double tol) =>
        Math.Abs(deg - 90.0) <= tol;

    // Angle en degrés au sommet B formé par (A-B-C)
    static double AngleDeg(Point A, Point B, Point C)
    {
        double ux = A.X - B.X, uy = A.Y - B.Y;
        double vx = C.X - B.X, vy = C.Y - B.Y;
        double du = Math.Sqrt(ux * ux + uy * uy);
        double dv = Math.Sqrt(vx * vx + vy * vy);
        if (du < 1e-6 || dv < 1e-6) return 0;
        double dot = ux * vx + uy * vy;
        double cos = Math.Max(-1.0, Math.Min(1.0, dot / (du * dv)));
        return Math.Acos(cos) * 180.0 / Math.PI;
    }

    // Ordonne les 4 coins en TL, TR, BR, BL
    static Point[] OrderCorners(Point[] pts)
    {
        // tri par y, puis x
        var sorted = pts.OrderBy(p => p.Y).ThenBy(p => p.X).ToArray();
        var top = sorted.Take(2).OrderBy(p => p.X).ToArray();
        var bot = sorted.Skip(2).OrderBy(p => p.X).ToArray();
        var tl = top[0]; var tr = top[1];
        var bl = bot[0]; var br = bot[1];
        return new[] { tl, tr, br, bl };
    }

    static double Dist(Point a, Point b)
    {
        double dx = a.X - b.X, dy = a.Y - b.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    // Largeur/hauteur moyens d’un quad ordonné (TL,TR,BR,BL)
    static (double w, double h) EstimateQuadSizePixels(Point[] q)
    {
        if (q == null || q.Length != 4) return (0, 0);
        var w = (Dist(q[0], q[1]) + Dist(q[3], q[2])) / 2.0;
        var h = (Dist(q[0], q[3]) + Dist(q[1], q[2])) / 2.0;
        return (w, h);
    }

    static double QuadArea(Point[] q)
    {
        // Aire du quadrilatère via triangulation
        return TriangleArea(q[0], q[1], q[2]) + TriangleArea(q[0], q[2], q[3]);
    }

    static double TriangleArea(Point a, Point b, Point c)
    {
        return Math.Abs(0.5 * ((a.X * (b.Y - c.Y)) + (b.X * (c.Y - a.Y)) + (c.X * (a.Y - b.Y))));
    }

    #endregion // bazar

    private Dictionary<Urn.Urn, double> _extractActualParameters(VideoCapture videoCapture)
    {
        CapProp[] properties = Enum.GetValues<CapProp>();
        Dictionary<Urn.Urn, double> result = new();

        foreach (CapProp property in properties)
        {
            string propertyName = Enum.GetName(property);
            if (propertyName is null)
                continue;
            Urn.Urn urn = new Urn.Urn($"urn:{propertyName}");
            if (result.ContainsKey(urn))
                continue;
            double value = videoCapture.Get(property);
            result.Add(urn, value);
        }
        return result;
    }

    public void StopCapture()
    {
        if (_videoCapture is null)
            return;

        _videoCapture.Stop();

        _videoCapture.Release();

        try
        {
            _videoCapture.Dispose();
        }
        catch (Exception)
        {
            _videoCapture.Dispose();
        }

        _videoCapture = null;

        GC.Collect();
        GC.WaitForPendingFinalizers();

        _cameraStatus = CameraStatus.Stopped;
    }

    private void _internalDispose()
    {

        if (_videoCapture is null)
            return;

        _videoCapture.Stop();
        try
        {
            _videoCapture.Dispose();
        }
        catch (Exception)
        {
            _videoCapture.Dispose();
        }
        _videoCapture = null;
    }
}
