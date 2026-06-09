namespace RIVS.ASAK.Core.Contract.DTO
{
    public class UserAccessDescriptorDTO
    {
        public int Result { get; set; }
        public string Message { get; set; }
        public string FullName { get; set; }
        public int? UserId { get; set; }
        public string UserRole { get; set; }

        public UserAccessDescriptorDTO()
        {
        }

    }

}
