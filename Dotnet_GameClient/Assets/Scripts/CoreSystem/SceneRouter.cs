using UnityEngine.SceneManagement;

namespace CoreSystem
{
    public static class SceneRouter
    {
        public const string LoadingScene = "LoadingScene";
        public const string LoginScene = "LoginScene";
        public const string MainScene = "MainScene";
        public const string TownScene = "TownScene";
        
        public static void Go(string sceneName) => SceneManager.LoadScene(sceneName);
    }
}