using WindowType = Data.WindowPublicContainer.Id;

namespace Services.WindowsService.Windows
{
    public readonly struct WindowRequest
    {
        public WindowType WindowType { get; }
        public WindowParams Params { get; }
        public int Priority { get; }

        public WindowRequest(WindowType windowType, WindowParams windowParams, int priority)
        {
            WindowType = windowType;
            Priority = priority;
            Params = windowParams;
        }
    }
}
