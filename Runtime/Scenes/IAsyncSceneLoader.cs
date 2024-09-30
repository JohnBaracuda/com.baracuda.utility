using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Baracuda.Utility.Scenes
{
    public interface IAsyncSceneLoader
    {
        public UniTask<Scene> LoadSceneAsync(int sceneBuildIndex, LoadSceneMode loadSceneMode);
    }
}