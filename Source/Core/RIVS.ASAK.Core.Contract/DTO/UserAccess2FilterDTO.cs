using System;

namespace RIVS.ASAK.Core.Contract.DTO
{

    public class UserAccess2FilterDTO
    {
        public string Login { get; set; }

        public string PasswordHash { get; set; }

        public Guid ActionId { get; set; }

        public Guid ObjectId { get; set; }
    }
}
