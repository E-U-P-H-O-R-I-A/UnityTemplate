using Cysharp.Threading.Tasks;
using Data.Scheme.Public;
using Services.WindowsService.Windows;

namespace Test
{
    public class NoAdsWindow : BaseWindow<BaseWindowParams>
    {
        public override WindowType Type => WindowType.NoAds;
    }
}