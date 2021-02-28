using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dropbox.Api;
using Dropbox.Api.Files;
using Dropbox.Api.Sharing;
using Dropbox.Api.Team;
using Dropbox.Api.Users;


namespace dotnetCore_dropbox
{
    class Program
    {
        // DropBox developer portal app's generated access token
        static readonly string _accessToken = "******";

        // path to log file
        static readonly string _logFolder = @"C:\dotnetCore_dropbox\";

        static bool _debug;
        static DateTime _timestamp;
        static string _timestampString;
        static string _logFile;
        static string _csvFile;
        static LogService _logService;
        static CsvGenerator<TeamMemberAccountModel> _csvTeamMemberAccount;
        static CsvGenerator<SharedFolderModel> _csvSharedFolder;
        static CsvGenerator<FolderMemberUserModel> _csvFolderMemberUser;
        static List<TeamMemberAccountModel> _teamMemberAccounts;
        static int _sharedFoldersListCount;
        static int _folderMemberUsersListCount;
        static string _csvString;
      

        static void Main(string[] args)
        {
            initApp();

            Task task = Task.Run((Func<Task>)Program.Run);

            task.Wait();
        }


        static void initApp()
        {
            _debug = true;

            if (_debug)
            {
                Console.WriteLine("-------------------------------------");
                Console.WriteLine("initApp() called...");
            }

            _timestamp = DateTime.Now;
            _timestampString = _timestamp.ToString("yyyyMMddHHmmss");
            _logFile = $@"{_logFolder}logs\{_timestampString}_dropbox_log.txt";

            _logService = new LogService(_logFile);
            _csvTeamMemberAccount = new CsvGenerator<TeamMemberAccountModel>(_logService, _debug, new TeamMemberAccountModel());
            _csvSharedFolder = new CsvGenerator<SharedFolderModel>(_logService, _debug, new SharedFolderModel());
            _csvFolderMemberUser = new CsvGenerator<FolderMemberUserModel>(_logService, _debug, new FolderMemberUserModel());

            _teamMemberAccounts = new List<TeamMemberAccountModel>();
        }        


        static async Task Run()
        {
            if (_debug)
            {
                Console.WriteLine("-------------------------------------");
                Console.WriteLine("Run() called...");
            }
            
            using (DropboxTeamClient teamClient = new DropboxTeamClient(_accessToken))
            {
                await ListTeamName(teamClient);
                
                await ListTeamMembers(teamClient);

                await EvaluateTeamMemberAccounts(teamClient);
            }
        }


        static async Task ListTeamName(DropboxTeamClient teamClient)
        {
            if (_debug)
            {
                Console.WriteLine("-------------------------------------");
                Console.WriteLine("ListTeamName() called...");
            }

            TeamGetInfoResult teamInfo = await teamClient.Team.GetInfoAsync();

            if (_debug)
            {
                Console.WriteLine($"teamInfo.Name: {teamInfo.Name}");
            }
        }


        static async Task ListTeamMembers(DropboxTeamClient teamClient)
        {   
            if (_debug)
            {
                Console.WriteLine("-------------------------------------");
                Console.WriteLine("ListTeamMembers() called...");
            }

            MembersListResult members = await teamClient.Team.MembersListAsync();
            
            int memberCount = 0;

            // no Count or Length, so have to enumerate to get count
            foreach (Dropbox.Api.Team.TeamMemberInfo member in members.Members)
            {
                if (_debug)
                {
                    Console.WriteLine("----------------");
                    Console.WriteLine($"Member ID: {member.Profile.TeamMemberId}");
                    Console.WriteLine($"Name     : {member.Profile.Name.DisplayName}");
                    Console.WriteLine($"Email    : {member.Profile.Email}");
                }

                memberCount++;
            }

            if (_debug)
            {
                Console.WriteLine("----------------");
                Console.WriteLine($"Total team member count           : {memberCount}");  // 252
                Console.WriteLine($"Any more members?  members.HasMore: {members.HasMore}");
            }

            while (members.HasMore)
            {
                if (_debug)
                {
                    Console.WriteLine("----------------");
                    Console.WriteLine($"members.HasMore: {members.HasMore}");
                    Console.WriteLine("Preparing to continue...");
                }
                
                MembersListResult membersContinue = await teamClient.Team.MembersListContinueAsync(members.Cursor);

                foreach (Dropbox.Api.Team.TeamMemberInfo member in members.Members)
                {
                    if (_debug)
                    {
                        Console.WriteLine("----------------");
                        Console.WriteLine($"Member ID: {member.Profile.TeamMemberId}");
                        Console.WriteLine($"Name     : {member.Profile.Name.DisplayName}");
                        Console.WriteLine($"Email    : {member.Profile.Email}");
                    }

                    memberCount++;
                }
            }

            if (_debug)
            {
                Console.WriteLine("----------------");
                Console.WriteLine($"Total team member count with continue: {memberCount}");  // 252   
            }
        }


        static async Task EvaluateTeamMemberAccounts(DropboxTeamClient teamClient)
        {   
            if (_debug)
            {
                Console.WriteLine("-------------------------------------");
                Console.WriteLine("EvaluateTeamMemberAccounts() called...");
            }
            
            MembersListResult teamMembers = await teamClient.Team.MembersListAsync();
            
            // sort team members by email, since name fields are user specified (typos, etc.)
            foreach (Dropbox.Api.Team.TeamMemberInfo teamMember in teamMembers.Members.OrderBy(m => m.Profile.Email))
            {
                _csvString = "";
                
                if (_debug)
                {
                    // // filter on test/validation account
                    // if (teamMember.Profile.Email == "test_user@your_company.com")
                    // {
                    //     await UpdateTeamMemberAccountList(teamClient, teamMember.Profile.TeamMemberId);
                    // }

                    // filter on account (validate shared folder owner)
                    if (teamMember.Profile.Email == "shared_folder_owner@your_company.com")
                    {
                        await UpdateTeamMemberAccountList(teamClient, teamMember.Profile.TeamMemberId);
                    }

                    // // filter on multiple associates
                    // // - one where ShareIsTeamFolder is true, the other where ShareIsInsideTeamFolder is true
                    // if (teamMember.Profile.Email == "target_user_1@your_company.com" || teamMember.Profile.Email == "target_user_2@your_company.com")
                    // {
                    //     await UpdateTeamMemberAccountList(teamClient, teamMember.Profile.TeamMemberId);
                    // }
                }
                else
                {
                   await UpdateTeamMemberAccountList(teamClient, teamMember.Profile.TeamMemberId);
                }
            }

            if (_debug)
            {
                Console.WriteLine("----------------");
                Console.WriteLine($"_teamMemberAccounts count: {_teamMemberAccounts.Count}");
            }
        }


        static async Task UpdateTeamMemberAccountList(DropboxTeamClient teamClient, string teamMemberId)
        {          
            if (_debug)
            {
                Console.WriteLine("-------------------------------------");
                Console.WriteLine("UpdateTeamMemberAccountList() called...");
            }

            TeamMemberAccountModel teamMemberAccount = await GetTeamMemberAccountDetails(teamClient, teamMemberId);

            OutputTeamMemberCsv(teamMemberAccount);

            _teamMemberAccounts.Add(teamMemberAccount);
        }


        static async Task<TeamMemberAccountModel> GetTeamMemberAccountDetails(DropboxTeamClient teamClient, string memberId)
        {
            if (_debug)
            {
                Console.WriteLine("-------------------------------------");
                Console.WriteLine("GetTeamMemberAccountDetails() called...");
            }

            TeamMemberAccountModel teamMemberAccount = await MapTeamMemberAccountDetail(teamClient, memberId);
            List<SharedFolderModel> teamMemberShares = await GetTeamMemberShares(teamClient, memberId);

            teamMemberAccount.SharedFolders = teamMemberShares;

            return teamMemberAccount;
        }


        static async Task<TeamMemberAccountModel> MapTeamMemberAccountDetail(DropboxTeamClient teamClient, string memberId)
        { 
            if (_debug)
            {
                Console.WriteLine("-------------------------------------");
                Console.WriteLine("MapTeamMemberAccountDetail() called...");
            }
            
            TeamMemberAccountModel teamMemberAccount = null;
            
            using (DropboxClient userClient = teamClient.AsMember(memberId))
            {
                FullAccount fullAccount = await userClient.Users.GetCurrentAccountAsync();

                teamMemberAccount = new TeamMemberAccountModel()
                {
                    Email = fullAccount.Email,
                    LastName = CheckValueForComma(fullAccount.Name.Surname),
                    FirstName = CheckValueForComma(fullAccount.Name.GivenName)
                };
            }

            if (_debug)
            {
                Console.WriteLine("----------------");
                Console.WriteLine($"Email              : {teamMemberAccount.Email}");
                Console.WriteLine($"LastName           : {teamMemberAccount.LastName}");
                Console.WriteLine($"FirstName          : {teamMemberAccount.FirstName}");
                Console.WriteLine($"SharedFolders.Count: going to figure that out next...");
            }

            return teamMemberAccount;
        }


        static async Task<List<SharedFolderModel>> GetTeamMemberShares(DropboxTeamClient teamClient, string memberId)
        { 
            if (_debug)
            {
                Console.WriteLine("-------------------------------------");
                Console.WriteLine("GetTeamMemberShares() called...");
            }
            
            // initialize here so sharedFolders in scope of return statement
            List<SharedFolderModel> sharedFolders = new List<SharedFolderModel>();

            _sharedFoldersListCount = 0;

            using (DropboxClient userClient = teamClient.AsMember(memberId))
            {
                ListFoldersResult sharedFoldersList = await userClient.Sharing.ListFoldersAsync();

                sharedFolders = await ProcessSharedFoldersList(userClient, sharedFoldersList);

                if (_debug)
                {
                    // before ...ContinueAsync() count
                    Console.WriteLine("----------------");
                    Console.WriteLine($"Total shared folders count: {_sharedFoldersListCount}");
                    Console.WriteLine($"Any more shared folders?  sharedFoldersList.Cursor: {sharedFoldersList.Cursor}");
                }

                // iterate over pages from userClient.Sharing.ListFoldersContinueAsync if list.Cursor is set
                while (sharedFoldersList.Cursor != null)
                {
                    if (_debug)
                    {
                        Console.WriteLine("----------------");
                        Console.WriteLine($"sharedFoldersList.Cursor not null: {sharedFoldersList.Cursor}");
                        Console.WriteLine("Preparing to continue...");
                    }
                    
                    // sharedFoldersList.Cursor flips to null once all folders are listed via ListFoldersContinueAsync()
                    sharedFoldersList = await userClient.Sharing.ListFoldersContinueAsync(sharedFoldersList.Cursor);

                    List<SharedFolderModel> sharedFoldersCursorList = await ProcessSharedFoldersList(userClient, sharedFoldersList);

                    sharedFolders.AddRange(sharedFoldersCursorList);

                    if (_debug)
                    {
                        // after ...ContinueAsync() count
                        Console.WriteLine("----------------");
                        Console.WriteLine($"Total shared folders count with continue: {_sharedFoldersListCount}");
                    }
                }
            }

            return sharedFolders;
        }


        static async Task<List<SharedFolderModel>> ProcessSharedFoldersList(DropboxClient userClient, ListFoldersResult listFoldersResult)
        {
            if (_debug)
            {
                Console.WriteLine("ProcessSharedFoldersList() called...");
            }

            List<SharedFolderModel> sharedFolders = new List<SharedFolderModel>();
            SharedFolderModel sharedFolder;
            
            foreach (SharedFolderMetadata sharedFolderMetadata in listFoldersResult.Entries)
            {
                if (_debug)
                {
                    Console.WriteLine($"sharedFolderMetadata.PathLower: {sharedFolderMetadata.PathLower}");
                }
                
                // filter out shared folder has \"Not added\" status (no path or parent folder specified)
                if (!String.IsNullOrEmpty(sharedFolderMetadata.PathLower))
                {
                    sharedFolder = await MapSharedFolderMetadata(userClient, sharedFolderMetadata);

                    _sharedFoldersListCount++;

                    sharedFolders.Add(sharedFolder);
                }
            }

            return sharedFolders;           
        }


        static async Task<SharedFolderModel> MapSharedFolderMetadata(DropboxClient userClient, SharedFolderMetadata sharedFolderMetadata)
        {
            if (_debug)
            {
                Console.WriteLine("-------------------------------------");
                Console.WriteLine("MapSharedFolderMetadata() called...");
                Console.WriteLine("----------------");
                Console.WriteLine($"SharedFolderId: {sharedFolderMetadata.SharedFolderId}");
                Console.WriteLine($"Name: {sharedFolderMetadata.Name}");
                Console.WriteLine($"PathLower: {sharedFolderMetadata.PathLower}");
                Console.WriteLine($"SharedFolderId: {sharedFolderMetadata.SharedFolderId}");
                Console.WriteLine($"ParentFolderName: {sharedFolderMetadata.ParentFolderName}");
                Console.WriteLine($"IsTeamFolder: {sharedFolderMetadata.IsTeamFolder}");
                Console.WriteLine($"IsInsideTeamFolder: {sharedFolderMetadata.IsInsideTeamFolder}");
            }

            SharedFolderModel sharedFolder = new SharedFolderModel()
            {
                SharedFolderId = sharedFolderMetadata.SharedFolderId,
                SharedFolderName = CheckValueForComma(sharedFolderMetadata.Name),
                SharePath = CheckValueForComma(sharedFolderMetadata.PathLower),  // only set if the folder is mounted
                SharedFolderOwner = await GetSharedFolderOwner(userClient, sharedFolderMetadata.SharedFolderId),
                SharedFolderFileCount = await GetSharedFolderFileCount(userClient, sharedFolderMetadata),
                ParentFolderName = CheckValueForComma(sharedFolderMetadata.ParentFolderName),
                ShareIsTeamFolder = sharedFolderMetadata.IsTeamFolder,
                ShareIsInsideTeamFolder = sharedFolderMetadata.IsInsideTeamFolder,
                SharedFolderUsers = await GetSharedFolderUsers(userClient, sharedFolderMetadata.SharedFolderId) /*,
                SharedFolderGroups = await GetSharedFolderGroups(userClient, sharedFolderMetadata.SharedFolderId),
                SharedFolderInvitees = await GetSharedFolderInvitees(userClient, sharedFolderMetadata.SharedFolderId) */
            };

            if (_debug)
            {
                Console.WriteLine("-------------------------------------");
                Console.WriteLine("MapSharedFolderMetadata() - Mapped Files Summary:");
                Console.WriteLine($"SharedFolderId         : {sharedFolder.SharedFolderId}");
                Console.WriteLine($"SharedFolderName       : {sharedFolder.SharedFolderName}");
                Console.WriteLine($"SharePath              : {sharedFolder.SharePath}");
                Console.WriteLine($"SharedFolderFileCount  : {sharedFolder.SharedFolderFileCount}");
                Console.WriteLine($"SharedFolderOwner      : {sharedFolder.SharedFolderOwner}");
                Console.WriteLine($"ParentFolderName       : {sharedFolder.ParentFolderName}");
                Console.WriteLine($"ShareIsTeamFolder      : {sharedFolder.ShareIsTeamFolder}");
                Console.WriteLine($"ShareIsInsideTeamFolder: {sharedFolder.ShareIsInsideTeamFolder}");
            }

            return sharedFolder;
        }


        static string CheckValueForComma(string value)
        {
            if (value == null)
            {
                return value;
            }
            
            if (value.Contains(","))
            {
                value = '"' + value + '"';
            }

            return value;
        }


        static async Task<string> GetSharedFolderOwner(DropboxClient userClient, string sharedFolderId)
        {
            if (_debug)
            {
                Console.WriteLine("-------------------------------------");
                Console.WriteLine("GetSharedFolderOwner() called...");
            }

            string sharedFolderOwner = "";

            SharedFolderMembers sharedFolderMembers = await userClient.Sharing.ListFolderMembersAsync(sharedFolderId);

            UserMembershipInfo userMembershipInfo = sharedFolderMembers.Users.FirstOrDefault(i => i.AccessType.IsOwner);

            if (userMembershipInfo != null)
            {
                sharedFolderOwner = userMembershipInfo.User.Email;
            }

            if (_debug)
            {
                // before ...ContinueAsync()
                Console.WriteLine("----------------");
                Console.WriteLine($"sharedFolderOwner before cursor check: {sharedFolderOwner}");
                Console.WriteLine($"Any more owners?  sharedFolderMembers.Cursor: {sharedFolderMembers.Cursor}");
            }

            while (sharedFolderMembers.Cursor != null)
            {
                if (_debug)
                {
                    Console.WriteLine("----------------");
                    Console.WriteLine($"sharedFolderMembers.Cursor not null: {sharedFolderMembers.Cursor}");
                    Console.WriteLine("Preparing to continue...");
                }
                
                // sharedFolderMembers.Cursor flips to null once all folders are listed via ListFolderMembersContinueAsync()
                sharedFolderMembers = await userClient.Sharing.ListFolderMembersContinueAsync(sharedFolderMembers.Cursor);

                userMembershipInfo = sharedFolderMembers.Users.FirstOrDefault(i => i.AccessType.IsOwner);

                if (userMembershipInfo != null)
                {
                    sharedFolderOwner = userMembershipInfo.User.Email;
                }

                if (_debug)
                {
                    // after ...ContinueAsync() count
                    Console.WriteLine("----------------");
                    Console.WriteLine($"sharedFolderOwner: {sharedFolderOwner}");
                }
            }

            return sharedFolderOwner;
        }


        static async Task<List<FolderMemberUserModel>> GetSharedFolderUsers(DropboxClient userClient, string sharedFolderId)
        {
            if (_debug)
            {
                Console.WriteLine("-------------------------------------");
                Console.WriteLine("GetSharedFolderUsers() called...");
            }
            
            SharedFolderMembers sharedFolderMembers = await userClient.Sharing.ListFolderMembersAsync(sharedFolderId);

            _folderMemberUsersListCount = 0;

            List<FolderMemberUserModel> sharedFolderMemberUsers = ProcessApiFolderMemberUsers(sharedFolderMembers);

            if (_debug)
            {
                // before ...ContinueAsync() count
                Console.WriteLine("----------------");
                Console.WriteLine($"Total shared folders members: {_folderMemberUsersListCount}");
                Console.WriteLine($"Any more shared folder members?  sharedFoldersList.Cursor: {sharedFolderMembers.Cursor}");
            }

            // iterate over pages from userClient.Sharing.ListFoldersContinueAsync if list.Cursor is set
            while (sharedFolderMembers.Cursor != null)
            {
                if (_debug)
                {
                    Console.WriteLine("----------------");
                    Console.WriteLine($"sharedFolderMembers.Cursor not null: {sharedFolderMembers.Cursor}");
                    Console.WriteLine("Preparing to continue...");
                }
                
                // sharedFolderMembers.Cursor flips to null once all folders are listed via ListFolderMembersContinueAsync()
                sharedFolderMembers = await userClient.Sharing.ListFolderMembersContinueAsync(sharedFolderMembers.Cursor);

                List<FolderMemberUserModel> sharedFolderMembersCursorList = ProcessApiFolderMemberUsers(sharedFolderMembers);

                if (_debug)
                {
                    Console.WriteLine($"_folderMemberUsersListCount: {_folderMemberUsersListCount}");
                }

                sharedFolderMemberUsers.AddRange(sharedFolderMembersCursorList);

                if (_debug)
                {
                    // after ...ContinueAsync() count
                    Console.WriteLine("----------------");
                    Console.WriteLine($"Total shared folder members count with continue: {_sharedFoldersListCount}");
                }
            }

            return sharedFolderMemberUsers;
        }


        static List<FolderMemberUserModel> ProcessApiFolderMemberUsers(SharedFolderMembers sharedFolderMembers)
        {
            if (_debug)
            {
                Console.WriteLine("-------------------------------------");
                Console.WriteLine("ProcessApiFolderMemberUsers() called...");
            }

            List<FolderMemberUserModel> folderMemberUsers = new List<FolderMemberUserModel>();
            FolderMemberUserModel folderMemberUser;

            List<UserMembershipInfo> apiFolderMemberUsers = sharedFolderMembers.Users.ToList();

            foreach (UserMembershipInfo apiFolderMemberUser in apiFolderMemberUsers)
            {
                folderMemberUser = MapFolderMemberUser(apiFolderMemberUser);

                _folderMemberUsersListCount++;

                folderMemberUsers.Add(folderMemberUser);
            }

            if (_debug)
            {
                Console.WriteLine("----------------");
                Console.WriteLine($"folderMemberUsers.Count: {folderMemberUsers.Count}");
            }

            return folderMemberUsers; 
        }


        static FolderMemberUserModel MapFolderMemberUser(UserMembershipInfo apiFolderMemberUser)
        {
            if (_debug)
            {
                Console.WriteLine("-------------------------------------");
                Console.WriteLine("MapFolderMemberUser() called...");
            }

            FolderMemberUserModel folderMemberUser = new FolderMemberUserModel()
            {
                FolderMemberUserAccountId = apiFolderMemberUser.User.AccountId,
                FolderMemberUserDisplayName = apiFolderMemberUser.User.DisplayName,
                FolderMemberUserEmail = apiFolderMemberUser.User.Email
            };

            if (_debug)
            {
                Console.WriteLine("--------");
                Console.WriteLine($"FolderMemberUserAccountId  : {folderMemberUser.FolderMemberUserAccountId}");
                Console.WriteLine($"FolderMemberUserEmail      : {folderMemberUser.FolderMemberUserEmail}");
                Console.WriteLine($"FolderMemberUserDisplayName: {folderMemberUser.FolderMemberUserDisplayName}");
            }

            return folderMemberUser;
        }


        // static async Task<int> GetSharedFolderFileCount(DropboxClient userClient, string folderPath)
        static async Task<int> GetSharedFolderFileCount(DropboxClient userClient, SharedFolderMetadata sharedFolderMetadata)
        {
            if (_debug)
            {
                Console.WriteLine("-------------------------------------");
                Console.WriteLine("GetSharedFolderFileCount() called...");
            }

            ListFolderResult listFolderResult = await userClient.Files.ListFolderAsync(sharedFolderMetadata.PathLower);

            int fileCount = listFolderResult.Entries.Count();

            if (_debug)
            {
                Console.WriteLine("----------------");
                Console.WriteLine($"fileCount: {fileCount}");
            }

            while (listFolderResult.HasMore)
            {
                listFolderResult = await userClient.Files.ListFolderContinueAsync(listFolderResult.Cursor);

                fileCount += listFolderResult.Entries.Count();

                if (_debug)
                {
                    Console.WriteLine("----------------");
                    Console.WriteLine($"updated fileCount: {fileCount}");
                }
            }

            return fileCount;
        }


        static void OutputTeamMemberCsv(TeamMemberAccountModel teamMemberAccount)
        {
            // reset & update file name for _csvFile
            _csvFile = $@"E:\Scratch\dotnetCore_dropbox\csvs\{teamMemberAccount.Email}_dropbox_shares_{_timestampString}.csv";

            // reset _csvString for current team member
            _csvString = "";

            // team member details
            _csvString += _csvTeamMemberAccount.CreateCsvHeaderString();
            _csvString += _csvTeamMemberAccount.CreateCsvString(new List<TeamMemberAccountModel>(){ teamMemberAccount });
            _csvString += Environment.NewLine;

            // shared folder details
            foreach (SharedFolderModel sharedFolder in teamMemberAccount.SharedFolders)
            {
                _csvString += _csvSharedFolder.CreateCsvHeaderString();
                _csvString += _csvSharedFolder.CreateCsvString(new List<SharedFolderModel>() { sharedFolder });
                _csvString += Environment.NewLine;

                // shared folder members
                _csvString += _csvFolderMemberUser.CreateCsvHeaderString();
                _csvString += _csvFolderMemberUser.CreateCsvString(sharedFolder.SharedFolderUsers);
                _csvString += Environment.NewLine;
            }

            _csvTeamMemberAccount.CreateCsvFile(_csvFile, _csvString);
        }

    }
}


// JV TBD - stub code for groups & invitees, if that becomes necessary
// - i.e. see which groups have access to shared folders, see which people have been invited

            // List<GroupMembershipInfo> apiFolderMemberGroups = folderMembers.Groups.ToList();
            // List<InviteeMembershipInfo> apiFolderMemberInvitees = folderMembers.Invitees.ToList();

            // foreach (GroupMembershipInfo apiFolderMemberGroup in apiFolderMemberGroups)
            // {
            //     // folderMemberGroup.Group.GroupId;
            //     // folderMemberGroup.Group.GroupName
            // }

            // foreach (InviteeMembershipInfo apiFolderMemberInvitee in apiFolderMemberInvitees)
            // {
            //     // folderMemberInvitee.Invitee.AsEmail.ToString();
            // }