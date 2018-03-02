using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Net.Mail;

namespace ExceptionLogParser
{
    class Parser
    {
        static void Main(string[] args)
        {
            try
            {
                List<Segment> segments = new List<Segment>();
                List<LogEntry> exceptions = new List<LogEntry>();
                List<Category> categories = Category.GetAvailableCategories();

                string fileFolderLocation = ConfigurationManager.AppSettings["FileFolderLocation"];
                int minimumEntriesInSegment = Convert.ToInt32(ConfigurationManager.AppSettings["MinimumEntriesInSegment"]);
                int maximumTimeInSegment = Convert.ToInt32(ConfigurationManager.AppSettings["MaximumTimeInSegment"]);

                DateTime dateToParse = DateTime.Today.AddDays(-1);
                if (args.Length > 0 && args[0] != "")
                {
                    try
                    {
                        dateToParse = DateTime.Parse(args[0]);
                    }
                    catch (Exception ex)
                    {
                        HandleException("Invalid date passed as an argument.  The passed argument was: " + args[0], ex);
                        return;
                    }
                }

                List<string> entries;
                try
                {
                    entries = openAndReadFile(fileFolderLocation, dateToParse);
                }
                catch (Exception ex)
                {
                    HandleException(ex.Message, ex);
                    return;
                }

                Segment segment = new Segment();
                foreach (string entry in entries)
                {
                    LogEntry logEntry = new LogEntry();
                    logEntry.ParseEntryTextForException(entry);
                    if (logEntry.isException)
                    {
                        exceptions.Add(logEntry);

                        logEntry.ParseExceptionTextForCategoryAndSource(ref categories);

                        if (!segment.EligibleEntry(logEntry, maximumTimeInSegment))
                        {
                            if (segment.EntryCount() >= minimumEntriesInSegment)
                                segments.Add(segment);
                            segment = new Segment();
                        }

                        segment.AddEntry(logEntry);
                    }
                }

                if (segment.EntryCount() >= minimumEntriesInSegment)
                    segments.Add(segment);

                if (exceptions.Count > 0)
                    ProcessOutput(exceptions, segments, categories, dateToParse);
                else
                    Console.WriteLine("No exceptions on this date.");

                int seconds = 1;
                while (true)
                {
                    if (Console.KeyAvailable)
                        break;

                    System.Threading.Thread.Sleep(1000);
                    if (seconds++ >= 60)
                        break;
                }
            }
            catch(Exception ex)
            {
                HandleException(ex.Message, ex);
                return;
            }
        }

        public static List<string> openAndReadFile(string fileFolderLocation, DateTime dateToParse)
        {
            if (!Directory.Exists(fileFolderLocation)) {
                throw new Exception("Invalid directory selected.");
            }

            List<string> fileNames = Directory.GetFiles(fileFolderLocation, "*" + dateToParse.ToString("yyyyMMdd") + "*").ToList();
            List<string> returnEntries = new List<string>();

            if( fileNames.Count == 1 )
            {
                FileStream stream = new FileStream(fileNames[0], FileMode.Open);
                StreamReader reader = new StreamReader(stream);

                return reader.ReadToEnd().Split(new[] { dateToParse.ToString("yyyy-MM-dd ") }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            else
            {
                if( fileNames.Count > 1 )
                    throw new Exception("Multiple (" + fileNames.Count + ") log files for date: " + dateToParse.ToString("MM/dd/yyyy"));
                else
                    throw new Exception("No log file found for date: " + dateToParse.ToString("MM/dd/yyyy"));
            }
        }

        public static void ProcessOutput(List<LogEntry> exceptions, List<Segment> segments, List<Category> categories, DateTime dateParsed)
        {
            string siteName = ConfigurationManager.AppSettings["SiteName"];
            string subject = "AlertWatch Exceptions Report for " + siteName + " on " + dateParsed.ToString("MM/dd/yyyy");

            StringBuilder outputString = new StringBuilder();
            outputString.Append(subject + "\r\n");
            outputString.Append("Total Exceptions:" + exceptions.Count() + "\r\n");

            if (segments.Count > 0)
            {
                outputString.Append("\r\n\r\n");
                outputString.Append("Segment Details" + "\r\n");
                outputString.Append("----------------------------------------------\r\n");
                for ( int i = 0; i < segments.Count; i ++)
                {
                    outputString.Append(segments[i].ToString(i+1));
                }
            }

            outputString.Append("\r\n\r\n");
            outputString.Append("Category Details" + "\r\n");
            outputString.Append("----------------------------------------------\r\n");
            foreach (Category category in categories)
            {
                var list = exceptions.Where(i => i.category.plainText == category.plainText);
                if (list.Count() > 0)
                {
                    outputString.Append("Category: " + category.plainText + " - Count: " + list.Count() + "\r\n");
                    var grouped = list.GroupBy(i => i.source);
                    foreach (var g in grouped)
                    {
                        outputString.Append("    Source: " + g.Key + " - Count: " + g.Count() + "\r\n");
                    }
                }
            }

            Email email = new Email();
            email.body = outputString.ToString();
            email.subject = subject;
            email.toaddresses = ConfigurationManager.AppSettings["SuccessEmail"];
            try
            {
                email.SendMail();
            }
            catch(Exception ex)
            {
                Console.Write(ex.Message + "\r\n" + ex.StackTrace);
            }

            Console.Write(outputString);
        }

        public static void HandleException(string message, Exception ex)
        {
            string siteName = ConfigurationManager.AppSettings["SiteName"];

            Email email = new Email();
            email.body = ex.Message + "\r\n\r\n" + ex.StackTrace;
            email.subject = "Site: " + siteName + " - Exception Report Parser Error: " + message;
            email.toaddresses = ConfigurationManager.AppSettings["ErrorEmail"];
            try
            {
                email.SendMail();
            }
            catch (Exception ex2)
            {
                Console.Write(ex2.Message + "\r\n" + ex2.StackTrace);
            }
        }
    }
}
