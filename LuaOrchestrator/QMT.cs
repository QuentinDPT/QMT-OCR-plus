
namespace LuaOrchestrator;

public static class QMT
{
    private static Dictionary<string, object> _bank = new Dictionary<string, object>();

    public static Dictionary<string, object> Bank => _bank;

    public static void LogInfo(string text)
    {
        Console.WriteLine(DateTime.Now.ToString("yyyyMMddHHmmssfffffff") + " [Info]  : " + text);
    }

    public static void LogWarning(string text)
    {
        Console.Write(DateTime.Now.ToString("yyyyMMddHHmmssfffffff"));

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(" [Warn]");
        Console.ResetColor();

        Console.WriteLine("  : " + text);
    }

    public static void LogError(string text)
    {
            Console.Write(DateTime.Now.ToString("yyyyMMddHHmmssfffffff"));

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(" [Error]");
            Console.ResetColor();

            Console.WriteLine(" : " + text);
    }

    public static void DisplayImage(string imageReference)
    {
        Console.WriteLine("Quelqu'un veut afficher l'image " + imageReference);
    }

    public static string GetImage()
    {
        string reference = Guid.NewGuid().ToString();

        //var e = ServiceLocator.Service.Locator.GetService(typeof(VideoStreamService)) as VideoStreamService;

        //Mat img = new Mat();
        // e.GetImage().CopyTo(img, 0);

        //_bank.Add(reference, img);

        Console.WriteLine("Fetch de l'image " + reference);

        return reference;
    }

    public static void ClientCommand(string jsCommand)
    {

    }

    public static void Wait(int time_in_ms)
    {
        Thread.Sleep(time_in_ms);
    }
}
