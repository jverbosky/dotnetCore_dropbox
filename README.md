# dotnetCore_dropbox
.NET Core (C#) console app that (1) pulls Dropbox share details via using Dropbox Business API and (2) formats &amp; outputs data for each account to CSV

-----

This is a "quick & dirty" app I was tapped to develop for a migration project - since the resulting CSVs served their purpose, it wasn't refined as much as I would normally do.

I didn't find much out on the regular sources for the Dropbox Business API (e.g. Stack Overflow, GitHub, Dropbox API documentation), so I'm just putting this out there to help anyone else who might be in the same situation.

Hope it helps someone, someday!

-----

To run:

1. update the '_accessToken' value in Program.cs with your own (i.e. the Dropbox developer protal app's generated access token)

2. update the '_logFolder' value in Program.cs with the dotnetCore_dropbox folder (wherever you downloaded & extracted it)

3. open a terminal (e.g. Command Prompt)

4. 'cd' to the dotnetCore_dropbox folder (wherever you downloaded & extracted it)

5.  use 'dotnet run' to run the app (output will be in the ./csvs folder)

Tip: use 'dotnet run > output.txt' to run with debug output dumped to a TXT file

------

Output Notes:

The app can take a while to generate the CSVs - just for reference, one account (which had 1,000+ shares) took ~30 minutes to create the CSV file.

The app is designed to output the CSVs progressively (as it pulls back each account’s details) - so if there's an issue, you can just pick up from the last account that had issues.

------

CSV File Notes:

Each CSV (e.g. user.account@company.com_dropbox_shares_20201012112845.csv) contains a list of active shared folders.

For each share, the following is listed:

- share owner (SharedFolderOwner)
  - e.g. user.account@company.com, external.account@somewhere_else.com

- number of files (SharedFolderFileCount)
  -  e.g. 3, 190, 4

- share member details
  - i.e. FolderMemberUserAccountId, FolderMemberUserDisplayName, FolderMemberUserEmail values

-----

updated for GitHub 2021/02/28 - jv
