namespace Scripts
{
    /// <summary>
    /// Interface for resetting an object to 'after Initialize()' state. Do not use this interface to reset to other states, 
    /// and do not reset InInitialized in this interface.
    /// </summary>
    public interface IResetable
    {
        void ResetState();
    }
}