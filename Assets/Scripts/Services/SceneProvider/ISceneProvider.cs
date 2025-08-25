using Cysharp.Threading.Tasks;

namespace Services.SceneProvider
{
    public interface ISceneProvider
    {
        UniTask Load(string sceneName);
    }
}