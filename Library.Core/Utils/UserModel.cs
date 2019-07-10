
namespace WD.Library.Core
{
    public class UserModel
    {
        public uint UserId { get; set; }
        public string UserName { get; set; }
        public string UserPwd { get; set; }
        public bool AutoLogin { get; set; }
        public bool RememberPassoword { get; set; }        
    }
}
