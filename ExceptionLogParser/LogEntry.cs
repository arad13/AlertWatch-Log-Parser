using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionLogParser
{
    class LogEntry
    {
        public Category category;
        public DateTime time;
        public bool isException;
        public string exceptionDetails;
        public string source;

        public LogEntry(){
            category = new Category();
        }

        public bool ParseEntryTextForException(string entryText)
        {
            string[] details = entryText.Split('|');

            time = DateTime.Parse(details[0]);
            exceptionDetails = details[2];
            isException = !Int32.TryParse(exceptionDetails, out int k) && exceptionDetails.Contains("Exception");

            return isException;
        }

        public void ParseExceptionTextForCategoryAndSource(ref List<Category> availableCategories)
        {
            string[] exceptionStack = exceptionDetails.Split(new string[] { "\r\n   at " }, StringSplitOptions.RemoveEmptyEntries);

            if( availableCategories.Exists(i => i.rawText == exceptionStack[0]))
            {
                category = availableCategories.Find(i => i.rawText == exceptionStack[0]);
            }
            else
            {
                Category newCategory = new Category();
                newCategory.SetText(exceptionStack[0], exceptionStack[0]);
                category = newCategory;
                availableCategories.Add(newCategory);
                try
                {
                    Category.AddCategoryToConfigFile(newCategory.rawText, newCategory.plainText);
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }

            source = exceptionStack[exceptionStack.Length - 1].Replace("\r\n", "");
        }
    }
}
