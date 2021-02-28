using System;


namespace dotnetCore_dropbox
{
    [Serializable]
    public class FolderMemberUserModel
    {
        public string FolderMemberUserAccountId { get; set; }
        public string FolderMemberUserDisplayName { get; set; }
        public string FolderMemberUserEmail { get; set; }
    }
}