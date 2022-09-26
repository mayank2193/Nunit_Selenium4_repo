using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using NUnit.Framework;
using System.Threading;
using System.Collections.Generic;
using OpenQA.Selenium.Chrome;

namespace NUnitSelenium
{
    [TestFixture("android", "Pixel 3", "9")]
    [TestFixture("android", "Galaxy A12", "10")]
    [Parallelizable(ParallelScope.Children)]
    public class NUnitSeleniumSample
    {
        public static string LT_USERNAME = Environment.GetEnvironmentVariable("LT_USERNAME") ==null ? "your username" : Environment.GetEnvironmentVariable("LT_USERNAME");
        public static string LT_ACCESS_KEY = Environment.GetEnvironmentVariable("LT_ACCESS_KEY") == null ? "your accessKey" : Environment.GetEnvironmentVariable("LT_ACCESS_KEY");
        public static bool tunnel = Boolean.Parse(Environment.GetEnvironmentVariable("LT_TUNNEL")== null ? "false" : Environment.GetEnvironmentVariable("LT_TUNNEL"));       
        public static string build = Environment.GetEnvironmentVariable("LT_BUILD") == null ? "your build name" : Environment.GetEnvironmentVariable("LT_BUILD");
        public static string seleniumUri = "https://"+ LT_USERNAME+":" + LT_ACCESS_KEY+"@mobile-hub.lambdatest.com/wd/hub";


        ThreadLocal<IWebDriver> driver = new ThreadLocal<IWebDriver>();
        private String platformName;
        private String deviceName;
        private String platformVersion;

        public NUnitSeleniumSample(String platformName, String deviceName, String platformVersion)
        {
            this.platformName = platformName;
            this.deviceName = deviceName;
            this.platformVersion = platformVersion;
        }

        [SetUp]
        public void Init()

        {
            ChromeOptions capabilities = new ChromeOptions();
            Dictionary<string, object> ltOptions = new Dictionary<string, object>();
            ltOptions.Add("w3c", true);
            ltOptions.Add("platformName", platformName);
            ltOptions.Add("deviceName", deviceName);
            ltOptions.Add("platformVersion", platformVersion);
            ltOptions.Add("isRealMobile", true);
            ltOptions.Add("build", "Nunit Sample Repo");
            ltOptions.Add("username", LT_USERNAME);
            ltOptions.Add("accessKey", LT_ACCESS_KEY);
            ltOptions.Add("name", String.Format("{0}:{1}",
               TestContext.CurrentContext.Test.ClassName,
               TestContext.CurrentContext.Test.MethodName));
            capabilities.AddAdditionalOption("lt:options", ltOptions);
           
            driver.Value = new RemoteWebDriver(new Uri(seleniumUri), capabilities.ToCapabilities(), TimeSpan.FromSeconds(600));
            Console.Out.WriteLine(driver);
        }

        [Test]
       public void Todotest()
        {
            {
                Console.WriteLine("Navigating to todos app.");
                driver.Value.Navigate().GoToUrl("https://lambdatest.github.io/sample-todo-app/");

                driver.Value.FindElement(By.Name("li4")).Click();
                Console.WriteLine("Clicking Checkbox");
                driver.Value.FindElement(By.Name("li5")).Click();


                // If both clicks worked, then te following List should have length 2
                IList<IWebElement> elems = driver.Value.FindElements(By.ClassName("done-true"));
                // so we'll assert that this is correct.
                Assert.AreEqual(2, elems.Count);

                Console.WriteLine("Entering Text");
                driver.Value.FindElement(By.Id("sampletodotext")).SendKeys("Yey, Let's add it to list");
                driver.Value.FindElement(By.Id("addbutton")).Click();


            }
        }

        [TearDown]
        public void Cleanup()
        {
            bool passed = TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Passed;
            try
            {
                // Logs the result to LambdaTest
                ((IJavaScriptExecutor)driver.Value).ExecuteScript("lambda-status=" + (passed ? "passed" : "failed"));
            }
            finally
            {
                
                // Terminates the remote webdriver session
                driver.Value.Quit();
            }
        }
    }
}
