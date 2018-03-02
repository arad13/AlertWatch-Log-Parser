using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionLogParser
{
    class Segment
    {
        private List<LogEntry> logEntries;
        private DateTime beginTime;
        private DateTime lastTimeSeen;
        
        public Segment() {
            logEntries = new List<LogEntry>();
            lastTimeSeen = new DateTime();
        }

        public int EntryCount()
        {
            return logEntries.Count;
        }

        public void AddEntry(LogEntry entry)
        {
            logEntries.Add(entry);
            if (lastTimeSeen == new DateTime())
                beginTime = entry.time;
            lastTimeSeen = entry.time;
        }

        public bool EligibleEntry(LogEntry entry, int maximumTimeInSegment)
        {
            if (lastTimeSeen == new DateTime() || entry.time <= beginTime.AddSeconds(maximumTimeInSegment))
                return true;
            else
                return false;
        }

        public string ToString(int outputNumber)
        {
            return "Segment " + outputNumber + " had " + logEntries.Count + " exceptions beginning at: " + beginTime.ToString("hh:mm:ss") + "\n";
        }
    }
}
