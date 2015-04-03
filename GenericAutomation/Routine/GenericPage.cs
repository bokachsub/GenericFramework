using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.PageObjects;
using System.Threading;
using System.Collections.ObjectModel;
using OpenQA.Selenium.Support.UI;

namespace GenericAutomation
{
    public class GenericPage
    {
        protected IWebDriver driver;        

        public GenericPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        public void Sleep(int seconds)
        {
            Thread.Sleep(seconds * 1000);
        }

        //TD: Add CLASS_NAME_SUFFIX, CLASS_NAME_PREFIX, CLASS_NAME_CONTAINS, same for other selectors
        public virtual ReadOnlyCollection<IWebElement> GetElements(FindBy findBy, string selectorText)
        {
            ReadOnlyCollection<IWebElement> elements = null;
            switch (findBy)
            {
                case FindBy.CSS_SELECTOR:
                    elements = this.driver.FindElements(By.CssSelector(selectorText));
                    break;
                case FindBy.ID:
                    elements = this.driver.FindElements(By.Id(selectorText));
                    break;
                case FindBy.CLASS_NAME:
                    elements = this.driver.FindElements(By.ClassName(selectorText));
                    break;
                case FindBy.LINK_TEXT:
                    elements = this.driver.FindElements(By.LinkText(selectorText));
                    break;
                case FindBy.PARTIAL_LINK_TEXT:
                    elements = this.driver.FindElements(By.PartialLinkText(selectorText));
                    break;
                case FindBy.NAME:
                    elements = this.driver.FindElements(By.Name(selectorText));
                    break;
                case FindBy.TAGNAME:
                    elements = this.driver.FindElements(By.TagName(selectorText));
                    break;
                case FindBy.XPATH:
                    elements = this.driver.FindElements(By.XPath(selectorText));
                    break;
            }

            if (elements.Count == 0)
            {
                elements = null;
            }

            return elements;
        }


        public virtual bool ElementPresent(FindBy findBy, string selectorText)
        {
            bool isPresent = false;
            ReadOnlyCollection<IWebElement> elements = this.GetElements(findBy, selectorText);
            if (elements != null)
            {
                isPresent = true;
            }
            return isPresent;
        }

        public virtual bool ElementEnabled (FindBy findBy, string selectorText)
        {
            bool isEnabled = false;
            ReadOnlyCollection<IWebElement> elements = this.GetElements(findBy, selectorText);
            if (elements != null)
            {
                foreach (IWebElement element in elements)
                {
                    if (element.Enabled)
                    {
                        isEnabled = true;
                        break;
                    }
                }
            }

            return isEnabled;
        }

        public virtual bool ElementDisplayed(FindBy findBy, string selectorText)
        {
            bool isDisplayed = false;
            ReadOnlyCollection<IWebElement> elements = this.GetElements(findBy, selectorText);
            if (elements != null)
            {
                foreach (IWebElement element in elements)
                {
                    if (element.Displayed)
                    {
                        isDisplayed = true;
                        break;
                    }
                }
            }

            return isDisplayed;
        }

        public virtual bool Click(FindBy findBy, string selectorText, int timeout=30, int clickIndex = 0)
        {
            bool clickPass = false;
            ReadOnlyCollection<IWebElement> elements = null;
              
            bool isPresent = this.WaitForElementVisible(findBy, selectorText, timeout);
            if (isPresent)
            {             
                elements = this.GetElements(findBy, selectorText);
                elements[clickIndex].Click();
                clickPass = true;
            }            
            return clickPass;
        } //Click

        public virtual void DoubleClick(FindBy findBy, string selectorText, int selectorNumber = 0)
        {
            ReadOnlyCollection<IWebElement> elements = this.GetElements(findBy, selectorText);
            if (elements != null && elements.Count != 0)
            {
                Actions action = new Actions(driver);
                elements[0].SendKeys(""); // it works with this here. Take it out and double click stops working ... aliens
                action.DoubleClick(elements[0]).Perform();
            }
        } //DoubleClick

        public virtual bool MoveToElement(FindBy findBy, string selectorText, int timeout = 30, int index = 0)
        {
            bool hoverPass = false;
            ReadOnlyCollection<IWebElement> elements = null;
            try
            {                
                bool isPresent = this.WaitForElementVisible(findBy, selectorText, timeout);

                if (isPresent)
                {                    
                    elements = this.GetElements(findBy, selectorText);

                    Actions actions = new Actions(driver);
                    actions.MoveToElement(elements[index]);
                    actions.Perform();

                    hoverPass = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetType());
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            return hoverPass;
        }

        public virtual bool EnterTextWithValidation(FindBy findBy, string selectorText, string textToEnter, int timeout=30, int elementIndex = 0)
        {
            bool isMatch = false;

            try
            {
                string textToValidate = string.Empty;
                ReadOnlyCollection<IWebElement> elements = null;

                // wait for the element to appear as sometimes the page hasn't fully loaded.
                bool isPresent = this.WaitForElementVisible(findBy, selectorText, timeout);
                if (isPresent)
                {
                    elements = this.GetElements(findBy, selectorText);
                    elements[elementIndex].SendKeys(textToEnter);

                    switch (findBy)
                    {
                        case FindBy.CLASS_NAME:
                            textToValidate = this.driver.FindElements(By.ClassName(selectorText))[elementIndex].GetAttribute("value");
                            break;
                        case FindBy.CSS_SELECTOR:
                            textToValidate = this.driver.FindElements(By.CssSelector(selectorText))[elementIndex].GetAttribute("value");
                            break;
                        case FindBy.ID:
                            textToValidate = this.driver.FindElements(By.Id(selectorText))[elementIndex].GetAttribute("value");
                            break;
                        case FindBy.LINK_TEXT:
                            textToValidate = this.driver.FindElements(By.LinkText(selectorText))[elementIndex].GetAttribute("value");
                            break;
                        case FindBy.NAME:
                            textToValidate = this.driver.FindElements(By.Name(selectorText))[elementIndex].GetAttribute("value");
                            break;
                        case FindBy.PARTIAL_LINK_TEXT:
                            textToValidate = this.driver.FindElements(By.PartialLinkText(selectorText))[elementIndex].GetAttribute("value");
                            break;
                        case FindBy.TAGNAME:
                            textToValidate = this.driver.FindElements(By.TagName(selectorText))[elementIndex].GetAttribute("value");
                            break;
                        case FindBy.XPATH:
                            textToValidate = this.driver.FindElements(By.XPath(selectorText))[elementIndex].GetAttribute("value");
                            break;
                    }

                    if (textToValidate == textToEnter)
                    {
                        isMatch = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetType());
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            return isMatch;
        } //EnterTextWithValidation

        public virtual bool WaitForElementVisible(FindBy findBy, string selectorText, int secondsToWait, int selectorNumber=0)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(secondsToWait));

            switch(findBy)
            { 
                case FindBy.CLASS_NAME:
                    wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName(selectorText)));
                    break;
                case FindBy.CSS_SELECTOR:
                    wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName(selectorText)));
                    break;
                case FindBy.ID:
                    wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName(selectorText)));
                    break;
                case FindBy.LINK_TEXT:
                    wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName(selectorText)));
                    break;
                case FindBy.NAME:
                    wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName(selectorText)));
                    break;
                case FindBy.PARTIAL_LINK_TEXT:
                    wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName(selectorText)));
                    break;
                case FindBy.TAGNAME:
                    wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName(selectorText)));
                    break;
                case FindBy.XPATH:
                    wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName(selectorText)));
                    break;                                
            }
            
            return true;
        } //WaitForElementVisible

        public virtual bool WaitForElementToDissappear(FindBy findBy, string selectorText, int secondsToWait=30)
        {
            bool isGone = false;
            int secondsWaited = 0;
            ReadOnlyCollection<IWebElement> elements = this.GetElements(findBy, selectorText);
            while (secondsWaited < secondsToWait)
            {
                this.Sleep(1);
                elements = this.GetElements(findBy, selectorText);
                if (elements == null || elements.Count == 0)
                {
                    isGone = true;
                    break;
                }
                secondsWaited++;
            }
            return isGone;
        } //WaitForElementToDissappear

        public virtual bool WaitForBaseUrl(string baseUrlToWaitFor, int secondsToWait = 30)
        {
            string currentUrl;
            bool isMatch = false;
            int secondsWaited = 0;
            
            while (secondsWaited < secondsToWait)
            {
                this.Sleep(1);
                currentUrl = driver.Url;
                if (currentUrl.Contains(baseUrlToWaitFor))
                {
                    isMatch = true;
                    break;
                }

                secondsWaited++;
            }

            return isMatch;
        } //WaitForBaseUrl

        public virtual string GetElementText(FindBy findBy, string selectorText, int secondsToWait = 30, int selectorNumber = 0)
        {
            string elementText = string.Empty;
            bool isPresent = this.WaitForElementVisible(findBy, selectorText, secondsToWait, selectorNumber);
            if (isPresent)
            {
                ReadOnlyCollection<IWebElement> elements = this.GetElements(findBy, selectorText);
                elementText = elements[selectorNumber].Text;
            }

            return elementText;
        } //GetElementText

        public virtual bool GetCheckboxState(FindBy findBy, string selectorText, int selectorNumber = 0)
        {
            ReadOnlyCollection<IWebElement> elements = this.GetElements(findBy, selectorText);
            string checkedState = string.Empty;
            checkedState = elements[selectorNumber].GetAttribute("checked");
            if (checkedState == "true" || checkedState == "checked")
            {
                return true;
            }
            return false;
        } //GetCheckboxState

        public virtual bool SetCheckboxState(FindBy findBy, string selectorText, bool desiredState, int selectorNumber = 0)
        {
            bool currentState = this.GetCheckboxState(findBy, selectorText, selectorNumber);
            if (currentState != desiredState)
            {
                bool clicked = this.Click(findBy, selectorText, selectorNumber);                
                currentState = this.GetCheckboxState(findBy, selectorText, selectorNumber);
                if (currentState != desiredState)
                {
                    return false;
                }
                return true;
            }
            return true;
        } //SetCheckboxState



    } //GenericPage  

    public enum FindBy
    {
        CSS_SELECTOR, ID, CLASS_NAME, LINK_TEXT, NAME, PARTIAL_LINK_TEXT, TAGNAME, XPATH
    }
} 
