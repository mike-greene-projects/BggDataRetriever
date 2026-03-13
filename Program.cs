using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Web;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using CsvHelper;

namespace BggDataRetriever
{
    class Program
    {
        private static ChromeDriver? _driver;
        private static readonly string _outPath = @"C:\temp";
        private static readonly string _outFile = "boardgames_ranks.csv";
        
        public static async Task Main()
        {
            Console.CancelKeyPress += OnExit;
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

            try
            {
                string envInfo = Environment.GetEnvironmentVariable("bgginfo") ?? "";
                string[] envArr = Base64.Decode(envInfo).Split(";");

                if (DoDataFetch(24))
                {
                    ChromeOptions options = new ChromeOptions();
                    options.AddArgument("--disable-blink-features=AutomationControlled");
                    options.AddArgument("--start-maximized");
                    options.AddArgument("--headless");
                    options.AddArgument("--no-sandbox");
                    options.AddArgument("--disable-gpu");
                    options.AddArgument(
                        "user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                        "AppleWebKit/537.36 (KHTML, like Gecko) " +
                        "Chrome/146.0.0.0 Safari/537.36"
                    );

                    _driver = new ChromeDriver(options);

                    WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));

                    _driver.Navigate().GoToUrl(envArr[2]);

                    wait.Until(d => d.FindElement(By.XPath("//button[text()='Sign In']"))).Click();

                    wait.Until(d => d.FindElement(
                        By.XPath("/html/body/div[1]/div/div/form/div[2]/div[1]/input")
                    )).SendKeys(envArr[0]);

                    wait.Until(d => d.FindElement(
                        By.XPath("/html/body/div[1]/div/div/form/div[2]/div[2]/input")
                    )).SendKeys(envArr[1]);

                    wait.Until(d => d.FindElement(
                        By.XPath("/html/body/div[1]/div/div/form/div[3]/button")
                    )).Click();

                    wait.Until(d => d.Url.Contains("boardgamegeek"));

                    var link = wait.Until(d =>
                        d.FindElement(By.CssSelector("a[href*='X-Amz-Signature']"))
                    ).GetAttribute("href");

                    string amzDate = HttpUtility.ParseQueryString(new Uri(link).Query)["X-Amz-Date"];
                    string dlFile = $"bg_ranks_{amzDate}.zip";


                    await DownloadFile(link, $@"{_outPath}\{dlFile}");
                    Console.WriteLine($@"Saved to {_outPath}\{dlFile}");

                    using (var archive = ZipFile.OpenRead($@"{_outPath}\{dlFile}"))
                    {
                        var entry = archive.GetEntry(_outFile);
                        entry?.ExtractToFile(Path.Combine(_outPath, _outFile), true);
                    }
                }

                using HttpClient client = new HttpClient();
                List<BggCsvToJson> records = LoadCsv($@"{_outPath}\{_outFile}");

                foreach (var batch in records.Chunk(5000))
                {
                    var json = JsonSerializer.Serialize(batch);
                    var response = await client.PostAsync(
                        "http://127.0.0.1:8080/update",
                        new StringContent(json, System.Text.Encoding.UTF8, "application/json")
                    );
                    
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }

                Console.WriteLine("Application completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                CleanupDriver();
            }
        }
        
        static bool DoDataFetch(int fromHours)
        {
            string fullPath = Path.Combine(_outPath, _outFile);
            Console.WriteLine(fullPath);
            if (File.Exists(fullPath))
            {
                DateTime lastWrite = File.GetLastWriteTimeUtc(fullPath);
                if (DateTime.UtcNow - lastWrite < TimeSpan.FromHours(fromHours))
                {
                    Console.WriteLine($"File '{_outFile}' exists and is less than {fromHours} hours old. Skipping fetch.");
                    return false; // file is present and recent, no fetch needed
                }
            }
            return true;
        }
        
        static async Task DownloadFile(string url, string path)
        {
            using HttpClient client = new HttpClient();
            byte[] bytes = await client.GetByteArrayAsync(url);
            await File.WriteAllBytesAsync(path, bytes);
        }
        
        public static List<BggCsvToJson> LoadCsv(string path)
        {
            using StreamReader reader = new StreamReader(path);
            using CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            List<BggCsvToJson> records = csv.GetRecords<BggCsvToJson>()
                .Select(r =>
                {
                    r.Name = Sanitize(r.Name);
                    r.YearPublished = NormalizeYear(r.YearPublished);

                    // sanitize other string fields if needed
                    return r;
                })
                .ToList();

            return records;
        }

        static string Sanitize(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            input = Regex.Replace(input, @"\p{C}+", "");   // remove control chars
            input = input.Normalize(NormalizationForm.FormKC);
            return input.Trim();
        }

        static string NormalizeYear(string input)
        {
            input = Sanitize(input);

            if (int.TryParse(input, out int year))
            {
                if (year >= 1900 && year <= 2100)
                    return year.ToString();
            }
            
            return "1900";  // fallback for invalid or out-of-range years
        }
        
        static void OnExit(object? sender, ConsoleCancelEventArgs e)
        {
            CleanupDriver();
        }

        static void OnProcessExit(object? sender, EventArgs e)
        {
            CleanupDriver();
        }

        static void CleanupDriver()
        {
            try
            {
                _driver?.Quit();
                _driver?.Dispose();
                _driver = null;
            }
            catch { }
        }
    }
}
