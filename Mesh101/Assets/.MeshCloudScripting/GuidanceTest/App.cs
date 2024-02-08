namespace CloudScripting.Sample
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Mesh.CloudScripting;

    public class App : IHostedService, IAsyncDisposable
    {
        #region Private Fields
        private readonly ICloudApplication _app;
        private readonly ILogger<App> _logger;
        #endregion Private Fields
        #region Public Constructors

        public App(ICloudApplication app, ILogger<App> logger)
        {
            _app = app;
            _logger = logger;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            await StopAsync(CancellationToken.None)
                .ConfigureAwait(false);

            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken token)
        {
            // Find the button
            Node button = _app.Scene.FindFirstChild("CheckButton");
            var buttonTransform = (TransformNode)button;
            var buttonInteractable = buttonTransform.FindFirstChild<InteractableNode>();

            // Find the indicator
            Node indicator = _app.Scene.FindFirstChild("Indicator");

            // Get the state property
            var isChecked = indicator["IsChecked"];
            var isCheckedStorage = indicator["IsCheckedStorage"];

            // Try to set the state
            isChecked.SetValue(true);
            isCheckedStorage.SetValue(true);

            // Add your app startup code here
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken token)
        {
            // Custom logic could be added here for user apps
            return Task.CompletedTask;
        }

        #endregion Public Methods
    }
}