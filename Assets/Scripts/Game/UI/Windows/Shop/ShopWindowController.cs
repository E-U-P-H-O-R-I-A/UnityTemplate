using Cysharp.Threading.Tasks;
using Services.LogService;
using Services.WindowsService.Windows;

namespace Game.UI.Windows.Shop
{
    public class ShopWindowController : WindowController<ShopWindowView, ShopWindowParams>
    {
        private readonly ILogService logService;

        public ShopWindowController(ShopWindowView view, ILogService logService) : base(view)
        {
            this.logService = logService;
        }

        protected override UniTask OnBeforeOpen(ShopWindowParams windowParams)
        {
            View.SetTitle(windowParams.Title);

            logService.Log($"[ShopWindowController] Opening with title '{windowParams.Title}'", LogCategory.Windows);

            return UniTask.CompletedTask;
        }
    }
}
