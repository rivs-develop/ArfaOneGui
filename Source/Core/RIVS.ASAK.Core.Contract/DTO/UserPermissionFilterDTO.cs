using System;

namespace RIVS.ASAK.Core.Contract.DTO
{
    public class UserPermissionFilterDTO
    {
        public Guid UserId { get; set; }

        public Guid ActionId { get; set; }

        public Guid ObjectId { get; set; }
    }
}
