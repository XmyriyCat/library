using Microsoft.AspNetCore.Identity;

namespace Library.Data.Models
{
    public class RefreshToken : IdentityUserToken<Guid>
    {
        public DateTime CreationDate { get; set; }

        public DateTime ExpirationDate { get; set; }
    }
}