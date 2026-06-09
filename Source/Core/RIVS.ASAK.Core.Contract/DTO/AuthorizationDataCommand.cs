using System;

namespace RIVS.ASAK.Core.Contract.DTO
{
    public sealed class AuthorizationDataCommand
    {
        public string Login { get; private set; }

        public string Password { get; private set; }
    }

    public sealed class AuthorizationFullDataCommand
    {
        public string Login { get; private set; }

        public string Password { get; private set; }

        public Guid ActionId { get; set; }

        public Guid ObjectId { get; set; }
    }

    public sealed class PermissionFullDataCommand
    {
        public Guid UserId { get; set; }

        public Guid ActionId { get; set; }

        public Guid ObjectId { get; set; }
    }

    public sealed class PermissionDataCommand
    {
        public Guid ActionId { get; set; }

    }
}
