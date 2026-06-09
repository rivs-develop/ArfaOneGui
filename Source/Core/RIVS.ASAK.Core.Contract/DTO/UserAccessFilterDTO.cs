using System;

namespace RIVS.ASAK.Core.Contract.DTO
{
    public class UserAccessFilterDTO
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public Guid ActionId { get; set; }

        public Guid ObjectId { get; set; }
    }
}
