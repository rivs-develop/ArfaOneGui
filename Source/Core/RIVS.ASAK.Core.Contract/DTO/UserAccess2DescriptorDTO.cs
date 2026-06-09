using System;

namespace RIVS.ASAK.Core.Contract.DTO
{
    public class UserAccess2DescriptorDTO
    {
        public int Result { get; set; }
        public string Message { get; set; }
        public string FullName { get; set; }
        public Guid? UserId { get; set; }
        public Guid? RoleId { get; set; }

        public UserAccess2DescriptorDTO()
        {
        }
    }

}
