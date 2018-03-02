# AlertWatch-Log-Parser

Project Summary:

Exception Log Parser is a stand-alone console application that can parse through a single AlertWatch log file and provide information about the number of exceptions for a date, their grouping into “segments” - clumps of exceptions all together - and breakdown into various categories and sources.  The parser is designed to be set up directly on a site server and run for a single client.  It has various configuration options detailed below.  The program is designed to be executed after midnight to parse through the previous day’s log, however it can also be run from the command line and passed in a single date argument to parse and report on a specific date.

Configuration Options:

SiteName - The name of the site, used for subject line of report only

FileFolderLocation - The location of the folder on the server where the log files are contained.

MinimumEntriesInSegment - The minimum amount of entries needed to create a segment.

MaximumTimeInSegment - Time is in seconds.  This time is calculated from the “first seen” exception of a segment.

SMTPHost - Email server host address

SMTPPort - Email server port

SenderAccountUserName - Account to send the emails from

SenderAccountPassword - Account password for the above UserName

FromAddress - The address to be specified in the “From” line of the email sent

SuccessEmail - Destination address(es) of a successful log parse report.  Multiple addresses can be specified by using either a ‘;’ or ‘,’ between each address.

ErrorEmail - Destination address(es) of where to send an email if there was an error during parser execution.  Can also have multiple email addresses, as above.



Categories Configuration:

The app.config file also contains a section for category definitions, called “categoriesConfigs.” This has an element collection called “categories.”  Each category can be specified by an “add” element with “plaintext” and “rawtext” attributes.  “Plain text” is the “simple English” translation of the “raw text” taken from the first line of the exception stack trace.  This config section will be dynamically added to by the program if it encounters a “raw text” it does not have a definition for in the “categoriesConfigs” section.  This will allow for population of common error types (i.e. SQL timeouts) and give the flexibility to add in plain text for future exceptions that have not yet occurred.

The categories breakdown in the report will group all exceptions that match the same “plain text” together, so if multiple exceptions are categorized as “SQL Timeout”, there will be a single category “SQL Timeout” with the count of all exceptions that fall into that category.

As an example, here might be the categoriesConfigs section after a parser run:
<categoriesConfigs>
    <categories>
      <add plaintext="System.InvalidOperationException: There is already an open DataReader associated with this Command which must be closed first." rawtext="System.InvalidOperationException: There is already an open DataReader associated with this Command which must be closed first." />
      <add plaintext="System.InvalidOperationException: ExecuteReader requires an open and available Connection. The connection's current state is closed." rawtext="System.InvalidOperationException: ExecuteReader requires an open and available Connection. The connection's current state is closed." />
      <add plaintext="System.IO.IOException: The process cannot access the file 'c:\logs\AWOR.BJCAWORT01.AlertWatch.Beta-20180221.log' because it is being used by another process." rawtext="System.IO.IOException: The process cannot access the file 'c:\logs\AWOR.BJCAWORT01.AlertWatch.Beta-20180221.log' because it is being used by another process." />
    </categories>
</categoriesConfigs>

By replacing the “plaintext” attributes, the output of any exception matching the “rawtext” will be translated to the plain text:
<categoriesConfigs>
    <categories>
      <add plaintext="Improperly Closed DataReader" rawtext="System.InvalidOperationException: There is already an open DataReader associated with this Command which must be closed first." />
      <add plaintext="Closed Connection" rawtext="System.InvalidOperationException: ExecuteReader requires an open and available Connection. The connection's current state is closed." />
      <add plaintext="File in Use" rawtext="System.IO.IOException: The process cannot access the file 'c:\logs\AWOR.BJCAWORT01.AlertWatch.Beta-20180221.log' because it is being used by another process." />
    </categories>
</categoriesConfigs>




