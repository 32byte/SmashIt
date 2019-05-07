using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SmashIt
{
    public partial class Form1 : Form
    {
        private List<Activity> activities = new List<Activity>();
        public Form1()
        {
            InitializeComponent();
        }

        private void SmashIt(string key, string name, int count)
        {
            for(int i = 0; i < count; i++)
            {
                Activity a = new Activity();
                a.start(key, name + i);
                activities.Add(a);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            SmashIt(textBox1.Text, textBox2.Text, Convert.ToInt32(numericUpDown1.Value));
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            foreach (Activity a in activities)
            {
                a.stop();
            }
            activities.Clear();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (activities.Count == 0)
                return;
            foreach (Activity a in activities)
            {
                a.stop();
            }
        }
    }

    class Activity
    {
        private Thread thread;
        private IWebDriver webdriver;
        private bool running;

        public void start(string key, string name)
        {
            running = true;
            thread = new Thread(() =>
            {
                Random rnd = new Random();
                ChromeDriverService service = ChromeDriverService.CreateDefaultService();
                service.HideCommandPromptWindow = true;
                ChromeOptions options = new ChromeOptions();
                options.AddArgument("--headless");
                webdriver = new ChromeDriver(service, options);
                webdriver.Url = "https://kahoot.it";
                fillInKey(webdriver, key);
                fillInUser(webdriver, name);
                SwitchToIframe(webdriver, "gameBlockIframe");
                while (running)
                {
                    try
                    {
                        if (webdriver.FindElements(By.TagName("button")).Count != 0)
                            webdriver.FindElements(By.TagName("button"))[rnd.Next(webdriver.FindElements(By.TagName("button")).Count)].Click();
                    }
                    catch (Exception)
                    {/*In waiting for question...*/}
                }
            });
            thread.Start();
        }


        private void SwitchToIframe(IWebDriver webdriver, string iframe)
        {
            try
            {
                webdriver.SwitchTo().Frame(iframe);
            }
            catch (Exception)
            {
                if (!running)
                    return;
                SwitchToIframe(webdriver, iframe);
            }
        }

        private void fillInKey(IWebDriver webdriver, string key)
        {
            try
            {
                webdriver.FindElement(By.Id("inputSession")).SendKeys(key);
                Thread.Sleep(500);
                webdriver.FindElement(By.TagName("button")).Click();
            }
            catch (Exception)
            {
                if (!running)
                    return;
                fillInKey(webdriver, key);
            }
        }

        private void fillInUser(IWebDriver webdriver, string name)
        {
            try
            {
                webdriver.FindElement(By.Id("username")).SendKeys(name);
                Thread.Sleep(500);
                webdriver.FindElement(By.TagName("button")).Click();
            }
            catch (Exception)
            {
                if (!running)
                    return;
                fillInUser(webdriver, name);
            }
        }

        public void stop()
        {
            running = false;
            webdriver.Quit();
        }
    }
}
