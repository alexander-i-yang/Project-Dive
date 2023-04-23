using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace World
{
    public enum MyScenes
    {
        StartScene,
        Area1Scene,
    }
    
    public class LoadSceneManager : MonoBehaviour
    {
        [SerializeField] private string startSceneName;
        [SerializeField] private string area1SceneName;
        [SerializeField] private UnityEvent OnSceneStart;
        [SerializeField] private UnityEvent OnSceneEnd;

        private AsyncOperation asyncLoad;

        public Dictionary<MyScenes, string> ToSceneName() => new()
        {
            { MyScenes.StartScene, startSceneName },
            { MyScenes.Area1Scene, area1SceneName },
        };
        
        // Start is called before the first frame update
        void Start()
        {
            OnSceneStart?.Invoke();
        }

        //Referenced in Unity Events
        public void LoadArea1SceneAsync()
        {
            LoadSceneAsync(MyScenes.Area1Scene);
        }
        
        //Referenced in Unity Events
        public void LoadStartSceneAsync()
        {
            LoadSceneAsync(MyScenes.StartScene);
        }
        
        public void LoadSceneAsync(MyScenes s)
        {
            OnSceneEnd?.Invoke();
            StartCoroutine(LoadSceneAsyncRoutine(s));
        }

        private IEnumerator LoadSceneAsyncRoutine(MyScenes s)
        {
            yield return new WaitForSeconds(2);
            asyncLoad = SceneManager.LoadSceneAsync(ToSceneName()[s], LoadSceneMode.Single);
            asyncLoad.allowSceneActivation = true;
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }

        public void SwitchScene(MyScenes s)
        {
            // SceneManager.SetActiveScene(SceneManager.GetSceneByName(ToSceneName()[s]));
            asyncLoad.allowSceneActivation = true;
        }
    }
}