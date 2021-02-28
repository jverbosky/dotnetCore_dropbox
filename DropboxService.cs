using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
// using System.Net;  // support for HttpListener
using System.Net.Http;  // support for HttpClient
// using System.Text;
using System.Threading.Tasks;  // support for Task
// using System.Windows;

using Dropbox.Api;
// using Dropbox.Api.Common;
using Dropbox.Api.Files;
// using Dropbox.Api.Team;


namespace dotnetCore_dropbox
{
    public class DropboxService
    {



// JV - initially intended to create service class, but usage didn't support dev time
// - this is mainly scratch code I either pulled from other places or wrote/modified myself
// - it may work, "kind of" work or not work at all - just leaving it here for reference in case it might be useful later



        // public string AppName { get; private set; }
        // public string AuthenticationURL { get; private set; } 
        // public string AppKey { get; private set; }
        // public string AppSecret { get; private set; }
        // public string AccessTocken { get; private set; } 
        // public string Uid { get; private set; }

        // private DropboxClient DBClient;  
        // private ListFolderArg DBFolders;  
        // private string oauth2State;  
        
        // // Same as we have configured Under [Application] -> settings -> redirect URIs.  
        // private const string RedirectUri = "https://localhost/authorize";


        // public DropboxService(string ApiKey, string ApiSecret, string ApplicationName = "ApiPrototype")  
        // {  
        //     try  
        //     {  
        //         AppKey = ApiKey;  
        //         AppSecret = ApiSecret;  
        //         AppName = ApplicationName;  
        //     }  
        //     catch (Exception)  
        //     {  
  
        //         throw;  
        //     }  
        // }


        // public string GenerateAccessToken()  
        // {  
        //     try  
        //     {  
        //         string _strAccessToken = string.Empty;  
  
        //         if (CanAuthenticate())  
        //         {  
        //             if (string.IsNullOrEmpty(AuthenticationURL))  
        //             {  
        //                 throw new Exception("AuthenticationURL is not generated !");  
  
        //             }  
        //             Login login = new Login(AppKey, AuthenticationURL, this.oauth2State); // WPF window with Webbrowser control to redirect user for Dropbox login process.  
        //             login.Owner = Application.Current.MainWindow;  
        //             login.ShowDialog();  
        //             if (login.Result)  
        //             {  
        //                 _strAccessToken = login.AccessToken;  
        //                 AccessTocken = login.AccessToken;  
        //                 Uid = login.Uid;  
        //                 DropboxClientConfig CC = new DropboxClientConfig(AppName, 1);  
        //                 HttpClient HTC = new HttpClient();  
        //                 HTC.Timeout = TimeSpan.FromMinutes(10); // set timeout for each ghttp request to Dropbox API.  
        //                 CC.HttpClient = HTC;  
        //                 DBClient = new DropboxClient(AccessTocken, CC);  
        //             }  
        //             else  
        //             {  
        //                 DBClient = null;  
        //                 AccessTocken = string.Empty;  
        //                 Uid = string.Empty;  
        //             }  
        //         }  
  
        //         return _strAccessToken;  
        //     }  
        //     catch (Exception ex)  
        //     {  
        //         throw ex;  
        //     }  
        // }


        // public bool CanAuthenticate()  
        // {  
        //     try  
        //     {  
        //         if (AppKey == null)  
        //         {  
        //             throw new ArgumentNullException("AppKey");  
        //         }  
        //         if (AppSecret == null)  
        //         {  
        //             throw new ArgumentNullException("AppSecret");  
        //         }  
        //         return true;  
        //     }  
        //     catch (Exception)  
        //     {  
        //         throw;  
        //     }  
  
        // }




        // public bool FolderExists(string path)  
        // {  
        //     try  
        //     {  
        //         if (AccessTocken == null)  
        //         {  
        //             throw new Exception("AccessToken not generated !");  
        //         }  
        //         if (AuthenticationURL == null)  
        //         {  
        //             throw new Exception("AuthenticationURI not generated !");  
        //         }  
  
        //         var folders = DBClient.Files.ListFolderAsync(path);  
        //         var result = folders.Result;  
        //         return true;  
        //     }  
        //     catch (Exception ex)  
        //     {  
        //         return false;  
        //     }  
        // } 






//         // add an ApiKey (from https://www.dropbox.com/developers/apps) here
//         // private readonly string _apiKey = "testing,testing,1,2,3...";
//         private readonly string _apiKey = "baseline";

//         // loopback host is for demo purpose, needs to be an available port
//         private const string _loopbackHost = "http://127.0.0.1:52475/";
        
//         // URL to receive OAuth 2 redirect from Dropbox server
//         // - also need to register this redirect URL on https://www.dropbox.com/developers/apps
//         private readonly Uri _redirectUri = new Uri(_loopbackHost + "authorize");

//         // URL to receive access token from JS.
//         private readonly Uri _jsRedirectUri = new Uri(_loopbackHost + "token");


//         public DropboxService()
//         {
//             Console.WriteLine($"DropboxService _apiKey: {_apiKey}");
            
//             _apiKey = "testing,testing,1,2,3...";
            
//             Console.WriteLine($"updated _apiKey: {_apiKey}");
//         }





//         public async Task<int> Run()
//         {
//            DropboxCertHelper.InitializeCertPinning();

//            var accessToken = await this.GetAccessToken();

//            if (string.IsNullOrEmpty(accessToken))
//            {
//                return 1;
//            }

// // WebRequestHandler not supported in Core
// // - try without and see if works for our purposes (pulling folder data)
// // - https://stackoverflow.com/questions/61981331/web-request-handler-in-net-core-not-found

//         //    // Specify socket level timeout which decides maximum waiting time when no bytes are
//         //    // received by the socket.
//         //    var httpClient = new HttpClient(new WebRequestHandler { ReadWriteTimeout = 10 * 1000 })
//         //    {
//         //        // Specify request level timeout which decides maximum time that can be spent on
//         //        // download/upload files.
//         //        Timeout = TimeSpan.FromMinutes(20)
//         //    };

// // JV
//             var httpClient = new HttpClient();

//            try
//            {

// // JV - update SimpleTestApp with new app name
//                var config = new DropboxClientConfig("SimpleTestApp")
//                {
//                    HttpClient = httpClient
//                };

//                var client = new DropboxClient(accessToken, config);
               
//                await RunUserTests(client);

//                // Tests below are for Dropbox Business endpoints. To run these tests, make sure the ApiKey is for
//                // a Dropbox Business app and you have an admin account to log in.

// // JV - determine why userAgent property isn't recognized
//             //    var teamClient = new DropboxTeamClient(accessToken, userAgent: "SimpleTeamTestApp", httpClient: httpClient);

// // JV - maybe this will work?
//                var teamClient = new DropboxTeamClient(accessToken, "SimpleTeamTestApp", config);
//                await RunTeamTests(teamClient);
//            }
//            catch (HttpException e)
//            {
//                Console.WriteLine("Exception reported from RPC layer");
//                Console.WriteLine("    Status code: {0}", e.StatusCode);
//                Console.WriteLine("    Message    : {0}", e.Message);

//                if (e.RequestUri != null)
//                {
//                    Console.WriteLine("    Request uri: {0}", e.RequestUri);
//                }
//            }

//            return 0;
//         }


//         private async Task<string> GetAccessToken()
//         {
// /**
// per:
// https://www.dropbox.com/lp/developers/reference/oauth-guide

// Testing with a generated token

// If you'd like to quickly test out the Dropbox APIs using your own Dropbox account before implementing OAuth, 
// you can generate an access token from your newly created app in My apps by pressing the button that says 
// "Generate" in the OAuth 2 section of your app settings page.

// Keep in mind that this is only for your own account - you'll need to use the standard OAuth flow to obtain 
// access tokens for other users.

// Do not instruct your users to register their own Dropbox application to use your app - you just need to register 
// your app once. Your end-users will connect to that app via the OAuth flow.


// **/
//             // var accessToken = Settings.Default.AccessToken;

//             var accessToken = "";

//             // if (string.IsNullOrEmpty(accessToken))
//             // {
//                 try
//                 {
//                     Console.WriteLine("Waiting for credentials.");
//                     var state = Guid.NewGuid().ToString("N");
//                     var authorizeUri = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Token, _apiKey, _redirectUri, state: state);
//                     var http = new HttpListener();
//                     http.Prefixes.Add(_loopbackHost);

//                     http.Start();

//                     System.Diagnostics.Process.Start(authorizeUri.ToString());

//                     // Handle OAuth redirect and send URL fragment to local server using JS.
//                     await HandleOAuth2Redirect(http);

//                     // Handle redirect from JS and process OAuth response.
//                     var result = await HandleJSRedirect(http);

//                     if (result.State != state)
//                     {
//                         // The state in the response doesn't match the state in the request.
//                         return null;
//                     }

//                     Console.WriteLine("and back...");

//                     // // Bring console window to the front.
//                     // SetForegroundWindow(GetConsoleWindow());

//                     accessToken = result.AccessToken;
//                     var uid = result.Uid;
//                     Console.WriteLine("Uid: {0}", uid);

// // JV not clear if we'll need the uid
// // - if we do, will revisit this 
//                     // Settings.Default.AccessToken = accessToken;
//                     // Settings.Default.Uid = uid;

//                     // Settings.Default.Save();
//                 }
//                 catch (Exception e)
//                 {
//                     Console.WriteLine("Error: {0}", e.Message);
//                     return null;
//                 }
//             // }

//             return accessToken;
//         }


//         private async Task HandleOAuth2Redirect(HttpListener http)
//         {
//             var context = await http.GetContextAsync();

//             // We only care about request to RedirectUri endpoint.
//             while (context.Request.Url.AbsolutePath != _redirectUri.AbsolutePath)
//             {
//                 context = await http.GetContextAsync();
//             }

//             context.Response.ContentType = "text/html";

//             // Respond with a page which runs JS and sends URL fragment as query string
//             // to TokenRedirectUri.
//             using (var file = File.OpenRead("index.html"))
//             {
//                 file.CopyTo(context.Response.OutputStream);
//             }

//             context.Response.OutputStream.Close();
//         }


//         private async Task<OAuth2Response> HandleJSRedirect(HttpListener http)
//         {
//             var context = await http.GetContextAsync();

//             // We only care about request to TokenRedirectUri endpoint.
//             while (context.Request.Url.AbsolutePath != _jsRedirectUri.AbsolutePath)
//             {
//                 context = await http.GetContextAsync();
//             }

//             var redirectUri = new Uri(context.Request.QueryString["url_with_fragment"]);

//             var result = DropboxOAuth2Helper.ParseTokenFragment(redirectUri);

//             return result;
//         }






//         private async Task RunTeamTests(DropboxTeamClient client)
//         {
//            var members = await client.Team.MembersListAsync();

//            var member = members.Members.FirstOrDefault();

//            if (member != null)
//            {
//                // A team client can perform action on a team member's behalf. To do this,
//                // just pass in team member id in to AsMember function which returns a user client.
//                // This client will operates on this team member's Dropbox.
//                var userClient = client.AsMember(member.Profile.TeamMemberId);

//                await RunUserTests(userClient);
//            }
//         }



//         private async Task RunUserTests(DropboxClient client)
//         {
//            await GetCurrentAccount(client);

// // JV - update to valid path value
//            var path = "/DotNetApi/Help";

//         //    var folder = await CreateFolder(client, path);

//            var list = await ListFolder(client, path);

//         //    var firstFile = list.Entries.FirstOrDefault(i => i.IsFile);
//         //    if (firstFile != null)
//         //    {
//         //        await Download(client, path, firstFile.AsFile);
//         //    }


// // JV - need to review Dropbox business account
// // - do we use any team spaces?  if so, do we need to search it/them for shares?

//         //    var pathInTeamSpace = "/Test";
//         //    await ListFolderInTeamSpace(client, pathInTeamSpace);


//         //    await Upload(client, path, "Test.txt", "This is a text file");

//         //    await ChunkUpload(client, path, "Binary");
//         }



//         private async Task GetCurrentAccount(DropboxClient client)
//         {
//             var full = await client.Users.GetCurrentAccountAsync();

//             Console.WriteLine("Account id    : {0}", full.AccountId);
//             Console.WriteLine("Country       : {0}", full.Country);
//             Console.WriteLine("Email         : {0}", full.Email);
//             Console.WriteLine("Is paired     : {0}", full.IsPaired ? "Yes" : "No");
//             Console.WriteLine("Locale        : {0}", full.Locale);
//             Console.WriteLine("Name");
//             Console.WriteLine("  Display  : {0}", full.Name.DisplayName);
//             Console.WriteLine("  Familiar : {0}", full.Name.FamiliarName);
//             Console.WriteLine("  Given    : {0}", full.Name.GivenName);
//             Console.WriteLine("  Surname  : {0}", full.Name.Surname);
//             Console.WriteLine("Referral link : {0}", full.ReferralLink);

//             if (full.Team != null)
//             {
//                 Console.WriteLine("Team");
//                 Console.WriteLine("  Id   : {0}", full.Team.Id);
//                 Console.WriteLine("  Name : {0}", full.Team.Name);
//             }
//             else
//             {
//                 Console.WriteLine("Team - None");
//             }
//         }


//         private async Task<ListFolderResult> ListFolder(DropboxClient client, string path)
//         {
//             Console.WriteLine("--- Files ---");
//             var list = await client.Files.ListFolderAsync(path);

//             // show folders then files
//             foreach (var item in list.Entries.Where(i => i.IsFolder))
//             {
//                 Console.WriteLine("D  {0}/", item.Name);
//             }

//             foreach (var item in list.Entries.Where(i => i.IsFile))
//             {
//                 var file = item.AsFile;

//                 Console.WriteLine("F{0,8} {1}",
//                     file.Size,
//                     item.Name);
//             }

//             if (list.HasMore)
//             {
//                 Console.WriteLine("   ...");
//             }
//             return list;
//         }



//         private async Task<MembersListResult> ListTeamMembers(DropboxTeamClient client)
//         {
//             var members = await client.Team.MembersListAsync();

//             foreach (var member in members.Members)
//             {
//                 Console.WriteLine("Member id    : {0}", member.Profile.TeamMemberId);
//                 Console.WriteLine("Name         : {0}", member.Profile.Name);
//                 Console.WriteLine("Email        : {0}", member.Profile.Email);
//             }

//             return members;
//         }


    }
}


/**


per: https://csharp.hotexamples.com/examples/Dropbox.Api/DropboxClient/-/php-dropboxclient-class-examples.html





        public DropboxClientWrapper()
        {
            dropboxClient = new DropboxClient(Config.DropBoxAuthKey);

            dropboxHttpClient = new HttpClient();
            dropboxHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Config.DropBoxAuthKey);
        }


 public async Task GetAccount()
 {
     using (var client = new DropboxClient(AccessToken))
     {
         Account = await client.Users.GetCurrentAccountAsync();
     }
 }


        public static async Task<Dictionary<string, List<Tuple<string, string>>>> GetAllSharedLinks()
        {
            var linkList = new Dictionary<string, List<Tuple<string, string>>>();

            using (var dbx = new DropboxClient(DropboxAppAccessToken))
            {
                //var arg = new Dropbox.Api.Sharing.GetSharedLinksArg("folder");
                var list = await dbx.Sharing.GetSharedLinksAsync();

                foreach(var link in list.Links)
                {
                    var imgName = link.AsPath.Path.Substring(link.AsPath.Path.LastIndexOf('/') + 1);
                    string path = link.AsPath.Path.Replace("/" + imgName, "");

                    string rawUrl = link.Url.Substring(0, link.Url.Length - DropboxURLSuffix.Length) + RawURLSuffix;

                    if (path != string.Empty && imgName != "www.dropbox.com.url")
                    {
                        if (!linkList.ContainsKey(path))
                        {
                            linkList.Add(path, new List<Tuple<string, string>>());
                        }

                        linkList[path].Add(new Tuple<string, string>(rawUrl, imgName));
                    }
                }
            }

            return linkList;
        }




 public async Task<ActionResult> GetFiles(string folder)
 {
     using (var dropboxClient = new DropboxClient(Common.Constants.ServerConstants.DropboxClient))
     {
         var files = await ListUserFiles(dropboxClient, folder);
         return Json(files, JsonRequestBehavior.AllowGet);
     }
 }



public async Task<UserInfo> GetUserInfoAsync()
 {
     UserInfo user = new UserInfo();
     var config = new DropboxClientConfig() { HttpClient = client };
     using (var dbx = new DropboxClient(AccessToken, config))
     {
         var full = await dbx.Users.GetCurrentAccountAsync();
         user.Email = full.Email;
         user.Name = full.Name.DisplayName;
     }
     return user;
 }


        public async Task ListRootFolder(DropboxClient dbx)
        {
            var list = await dbx.Files.ListFolderAsync(string.Empty);

            // show folders, then files
            foreach (var item in list.Entries.Where(i => i.IsFolder))
            {
                Console.WriteLine("D  {0}/", item.Name);
            }

            foreach (var item in list.Entries.Where(i => i.IsFile))
            {
                Console.WriteLine("F{0,8} {1}", item.AsFile.Size, item.Name);
            }
        }



        private async Task<UserFilesViewModel> ListUserFiles(DropboxClient dropboxClient, string folder)
        {
            if (string.IsNullOrEmpty(folder))
            {
                folder = this.User.Identity.Name;
            }
            var list = await dropboxClient.Files.ListFolderAsync("/" + folder);
            
            var userFiles = new UserFilesViewModel();

            userFiles.Folders = list.Entries.Where(i => i.IsFolder).Select(i => i.Name).ToList();

            userFiles.Files = list.Entries.Where(i => i.IsFile).Select(i => i.Name).ToList();

            return userFiles;
        }







**/
