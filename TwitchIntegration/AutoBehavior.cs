using UnityEngine;

#pragma warning disable RCS1018, RCS1213, IDE0051
namespace TwitchIntegration
{
    public class AutoBehaviour : MonoBehaviour
    {
        public static GameObject Instance;

        public static void CreateInstance()
        {
            Instance = new GameObject();
            Instance.AddComponent<AutoBehaviour>();
            Instantiate(Instance);
        }

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void OnDestroy()
        {
            Plugin.AppRunning = false;
            Entry.Instance.CloseTwitchAPI();
        }
    }
}
