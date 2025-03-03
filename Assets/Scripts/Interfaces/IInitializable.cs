namespace Scripts 
{
    /// <summary>
    /// Interface needed to use instead constructors. Initialize() must be called once, and set IsInitialized true <br /> <br />
    /// Try use <see cref="Extensions.Extensions.CheckInitialization(ref bool)"/>
    /// </summary>
    public interface IInitializable
    {
        bool IsInitialized { get; }

        bool Initialize();
    }
}
