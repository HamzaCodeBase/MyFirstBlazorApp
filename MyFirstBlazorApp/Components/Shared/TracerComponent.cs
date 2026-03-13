using Microsoft.AspNetCore.Components;

namespace MyFirstBlazorApp.Components.Shared
{
    public class TracerComponent : ComponentBase, IDisposable
    {
        // Short per-instance id so you can tell which instance printed the line
        protected string Id { get; } = Guid.NewGuid().ToString("N")[..5];

        // Increments after each interactive render
        private int _interactiveRenderCount = 0;

        // Becomes true after the first interactive OnAfterRender fires
        private static bool _interactiveHasStarted = false;
        private static bool _prerenderCompleted = false;   // prerender instance(s) have been disposed
        private static bool _interactiveStarted = false;   // first interactive OnAfterRender has fired
        private static string CurrentPhaseLabel =>
            _interactiveStarted
                ? "⚡ INTERACTIVE"
                : _prerenderCompleted
                    ? "🚚 INTERACTIVE (pre-render)"
                    : "🧊 PRERENDER";

        private static string? _lastComponentName; // (keep your spacer helper)

        private void Log(string hook, bool? firstRender = null)
        {
            var name = GetType().Name;

            if (_lastComponentName != name)
            {
                Console.WriteLine("");
                _lastComponentName = name;
            }

            var ts = DateTime.Now.ToString("HH:mm:ss.fff");
            var fr = firstRender is null ? "" : $" firstRender={firstRender}";
            var rc = _interactiveStarted ? $" pass={_interactiveRenderCount}" : "";
            Console.WriteLine($"{ts} | {CurrentPhaseLabel} | {name}#{Id} | {hook}{fr}{rc}");
        }


        protected override void OnInitialized()
        {
            Log("OnInitialized");
        }

        protected override Task OnInitializedAsync()
        {
            Log("OnInitializedAsync");
            return Task.CompletedTask;
        }

        protected override void OnParametersSet()
        {
            Log("OnParametersSet");
        }

        protected override Task OnParametersSetAsync()
        {
            Log("OnParametersSetAsync");
            return Task.CompletedTask;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                _interactiveStarted = true;         // now fully interactive
            }
            _interactiveRenderCount++;
            Log("OnAfterRender", firstRender);
        }

        public void Dispose()
        {
            // If we’re disposing before the interactive phase has begun,
            // that means the prerender instance is being torn down.
            if (!_interactiveStarted)
            {
                _prerenderCompleted = true;         // next instance is the interactive one
            }
            Log("Dispose");
        }
    }

}
