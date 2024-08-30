using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace FileDeletionRecordService
{
    public sealed class FileDeletionEventLogCollectService
    {
        private readonly ILogger<TimedHostedService> _logger;


        public FileDeletionEventLogCollectService(ILogger<TimedHostedService> logger)
        {
            _logger = logger;
        }

        public void CollectDeletionRecordFromSystemEventLog()
        {
            // Set the path to the folder you want to check  
            string folderPath = "C:\\Users\\hsiaoc.AUTH\\Pictures\\";

            // Retrieve the Security Event Log  
            EventLog securityLog = new EventLog("Security");

            // Get the entries for file deletion events  
            EventLogEntryCollection entries = securityLog.Entries;

            // Iterate over the entries in reverse order to find the most recent deletion event  
            for (int i = entries.Count - 1; i >= 0; i--)
            {
                EventLogEntry entry = entries[i];

                // Check if the event is a file deletion event and matches the file path  
                if (entry.InstanceId == 4663 && entry.Message.Contains(folderPath))
                {
                    // Get the account name associated with the deletion event  
                    SecurityIdentifier sid = new SecurityIdentifier(entry.ReplacementStrings[0]);
                    NTAccount account = (NTAccount)sid.Translate(typeof(NTAccount));

                    _logger.LogWarning(@$" File : [{entry.ReplacementStrings[6]}] {Environment.NewLine} Deleted by : [{account.Value}] {Environment.NewLine} Deleted at : [{entry.TimeGenerated.ToString("yyyy-MM-dd HH:mm:ss")}]");
                }
            }
        }
    }
}
