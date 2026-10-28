namespace Radical.Avalonia.Bootstrap
{
    /// <summary>
    /// Determines the reason for application shutdown.
    /// </summary>
    public enum ApplicationShutdownReason
    {
        /// <summary>
        /// The application has been shutdown using the Radical canonical behaviors.
        /// In this case the shutdown process can be canceled.
        /// </summary>
        UserRequest = 0,

        /// <summary>
        /// The application is shutting down because another 
        /// instance is already running and the application 
        /// is marked as singleton.
        /// </summary>
        MultipleInstanceNotAllowed = 1,

        /// <summary>
        /// The application is shutting down because the operating system session is ending.
        /// </summary>
        /// <remarks>
        /// Avalonia does not expose a session ending notification; this value is kept
        /// for source compatibility with Radical.Windows and is currently never raised.
        /// </remarks>
        SessionEnding,

        /// <summary>
        /// The application has been shut down using the App.Current.Shutdown() method.
        /// </summary>
        ApplicationRequest,
    }
}
