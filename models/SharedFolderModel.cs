using System;
using System.Collections.Generic;


namespace dotnetCore_dropbox
{
    [Serializable]
    public class SharedFolderModel
    {
        public string SharedFolderId { get; set; }
        public string SharedFolderName { get; set; }
        public string SharePath { get; set; }
        public string SharedFolderOwner { get; set; }
        public int SharedFolderFileCount { get; set; }
        public string ParentFolderName { get; set; }
        public bool ShareIsTeamFolder { get; set; }
        public bool ShareIsInsideTeamFolder { get; set; }
        public List<FolderMemberUserModel> SharedFolderUsers { get; set; }
        public List<FolderMemberGroupModel> SharedFolderGroups { get; set; }
        public List<FolderMemberInviteeModel> SharedFolderInvitees { get; set; }
    }
}