using System;
using System.Collections.Generic;


namespace dotnetCore_dropbox
{
    [Serializable]
    public class TeamMemberAccountModel
    {
        public string Email { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public List<SharedFolderModel> SharedFolders { get; set; }
    }
}