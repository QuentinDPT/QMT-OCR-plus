using System.IO.MemoryMappedFiles;
using System.Text;

namespace ImageProducer
{
    internal class Program
    {
        public static string MMFToken { get; private set; }

        public static string MMFReadynessTocken => MMFToken + "Ready";

        public static string ProducerReady => MMFToken + "ProducerReady";

        public static string ConsumerConsumed => MMFToken + "ConsumerConsumed";

        public static int MMFSize;

        public static int ProcessPort;

        static void Main(string[] args)
        {
            Console.Title = "Data producer";

            // Pouvoir créer un process avec les données suivantes
            //  - UUID clé de mémoire partagée
            //  - int taille de la mémoire partagée
            //
            // Généré :
            //  - Readyness de la création de la mémoire partagée

            // Crée une mémoire partagée nommée
            MMFToken = args[0];
            MMFSize = int.Parse(args[1]);
            ProcessPort = int.Parse(args[2]);

            Console.WriteLine($"MMF \t\t= {MMFToken}");
            Console.WriteLine($"MMF Size \t= {MMFSize}");
            Console.WriteLine($"Process port \t= {ProcessPort}");

            using var mmf = MemoryMappedFile.CreateOrOpen(MMFToken, MMFSize);
            using var accessor = mmf.CreateViewAccessor();

            using var dataReady = new EventWaitHandle(false, EventResetMode.AutoReset, ProducerReady);
            using var dataProcessed = new EventWaitHandle(false, EventResetMode.AutoReset, ConsumerConsumed);

            using var readyEvent = EventWaitHandle.OpenExisting(MMFReadynessTocken);
            readyEvent.Set();

            Thread.Sleep(1000);

            while (true)
            {
                Console.WriteLine("Generating data...");
                byte[] bytes = Encoding.UTF8.GetBytes("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAA0AAAAOCAYAAAD0f5bSAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABrSURBVChTY/hPBmBAFyAGoGhiYMBuhr3HGRQ+XBVIAwwjA3QNIIDXJmwaQAC7e/BoAAGsmtA1vJVRQeFj1YQMQBpgGAYIagIBgjYpbfRBF8IAKJpAGmAYK6hXB2PCNkEVgjEUYGgiBpClCQA2eKTmnhYCAQAAAABJRU5ErkJggg==");

                // Écrit le message à l'offset 0
                accessor.WriteArray(0, bytes, 0, bytes.Length);

                dataReady.Set();
                Console.WriteLine("Data set, wait for consumer...");
                dataProcessed.WaitOne();
                Console.WriteLine("Data consumed !");
            }
        }
    }
}
