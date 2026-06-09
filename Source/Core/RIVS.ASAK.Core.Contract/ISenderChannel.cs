namespace RIVS.ASAK.Core.Contract
{
    public interface ISenderChannel
    {
        /// <summary>
        /// Sends specified data via channel.
        /// </summary>
        /// <param name="data">Data to send.</param>
        void SetData(object data);
    }
}
