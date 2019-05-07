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
        private List<Activity> threads = new List<Activity>();
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
                threads.Add(a);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            SmashIt(textBox1.Text, textBox2.Text, 1);
        }

        private void Button2_Click(object sender, EventArgs e)
        {

        }
    }

    class Activity
    {
        public Thread thread;
        private IWebDriver webdriver;

        public void start(string key, string name)
        {
            thread = new Thread(() =>
            {
                Random rnd = new Random();
                webdriver = new ChromeDriver("./");
                webdriver.Url = "https://kahoot.it";
                fillInKey(webdriver, key);
                fillInUser(webdriver, name);
                SwitchToIframe(webdriver, "gameBlockIframe");
                while (true)
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
                SwitchToIframe(webdriver, iframe);
            }
        }

        private void fillInKey(IWebDriver webdriver, string key)
        {
            try
            {
                webdriver.FindElement(By.Id("inputSession")).SendKeys(key);
                Thread.Sleep(100);
                webdriver.FindElement(By.TagName("button")).Click();
            }
            catch (Exception)
            {
                fillInKey(webdriver, key);
            }
        }

        private void fillInUser(IWebDriver webdriver, string name)
        {
            try
            {
                webdriver.FindElement(By.Id("username")).SendKeys(name);
                Thread.Sleep(100);
                webdriver.FindElement(By.TagName("button")).Click();
            }
            catch (Exception)
            {
                fillInUser(webdriver, name);
            }
        }

        public void stop()
        {
            //webdriver.Dis
        }
    }
}
