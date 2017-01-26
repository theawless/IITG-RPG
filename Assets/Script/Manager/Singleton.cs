using UnityEngine;

namespace Script.Manager
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance = GameObject.FindGameObjectWithTag("GameController").AddComponent<T>();

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
