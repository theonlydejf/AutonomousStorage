namespace Team.HobbyRobot.ASCS.Core
{
    /// <summary>
    /// Item that is stored in a storage
    /// </summary>
    public interface IStorageItem
    {
        /// <summary>
        /// ID of the storage item
        /// </summary>
        int ID { get; }
    }
}