using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Web;

namespace BggDataRetriever 
{
    class Program
    {
        static async Task Main()
        {
            try
            {
                string envInfo = Environment.GetEnvironmentVariable("bgginfo") ?? "";
                string[] envArr = Base64.Decode(envInfo).Split(";");
                
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
                using ChromeDriver driver = new ChromeDriver(options);

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                
                driver.Navigate().GoToUrl(envArr[2]);
                /*
                 * If the target site detects that we are using headless selenium,
                 * It will never render the sign-in button.
                 * The user-agent tells the page we are safe (for now, 2026/03/10).
                 *
                 * ((ITakesScreenshot)driver).GetScreenshot().SaveAsFile(@"C:\temp\headless.png");
                  *  Console.WriteLine("Screenshot saved to C:\\temp\\headless.png");
                 */

                
                wait.Until(d => d.FindElement(
                    By.XPath("//button[text()='Sign In']"))
                ).Click();
                
                wait.Until(d => d.FindElement(
                        By.XPath("/html/body/div[1]/div/div/form/div[2]/div[1]/input"))
                ).SendKeys(envArr[0]);
                
                wait.Until(d => d.FindElement(By.XPath("/html/body/div[1]/div/div/form/div[2]/div[2]/input"))
                ).SendKeys(envArr[1]);
                
                wait.Until(d => d.FindElement(
                    By.XPath("/html/body/div[1]/div/div/form/div[3]/button"))
                ).Click();
                
                wait.Until(d => d.Url.Contains("boardgamegeek"));
                
                // find the S3 download link
                var link = wait.Until(d =>
                    d.FindElement(By.CssSelector("a[href*='X-Amz-Signature']"))
                ).GetAttribute("href");

                string amzDate = HttpUtility.ParseQueryString(new Uri(link).Query)["X-Amz-Date"];
                string filePath = $@"C:\temp\bg_ranks_{amzDate}.csv";
                await DownloadFile(link, filePath);
                Console.WriteLine($"Saved to {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString()); 
            }
        }

        static async Task DownloadFile(string url, string path)
        {
            using HttpClient client = new HttpClient();
            byte[] bytes = await client.GetByteArrayAsync(url);
            await File.WriteAllBytesAsync(path, bytes);
        }
    }
}
