using System;

namespace Utility.MVC
{
    public interface IController : IDisposable
    {
        void Initialize();
    }
}
