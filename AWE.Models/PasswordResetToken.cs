#nullable disable
using System;

namespace AWE.Models
{
    public class PasswordResetToken
    {
        public int TokenID { get; set; }
        public int UserID { get; set; }
        public string Token { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UsedDate { get; set; }
    }
}
