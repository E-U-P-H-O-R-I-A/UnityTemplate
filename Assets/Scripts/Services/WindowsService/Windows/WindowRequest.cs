using Data.Scheme.Public;

namespace Services.WindowsService.Windows
{
    public readonly struct WindowRequest
    {
        public WindowType WindowType { get; }
        public BaseWindowParams Params { get; }
        public int Priority { get; }

        public WindowRequest(WindowType windowType, BaseWindowParams @params, int priority)
        {
            WindowType = windowType;
            Priority = priority;
            Params = @params;
        }
    }
}
