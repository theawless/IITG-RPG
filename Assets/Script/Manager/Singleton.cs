using UnityEngine;

namespace Script.Manager
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static readonly object lockObject = new object();
        private static readonly GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
        private static T instance = gameController.AddComponent<T>();

        public static T Instance
        {
            get { return instance; }
        }

        private void OnApplicationQuit()
        {
            instance = null;
        }
    }
}
