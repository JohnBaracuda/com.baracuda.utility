using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Baracuda.Bedrock.Scenes
{
    public interface IAsyncSceneLoader
    {
        public UniTask<Scene> LoadSceneAsync(int sceneBuildIndex, LoadSceneMode loadSceneMode);
    }
}