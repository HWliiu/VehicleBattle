using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameClient.Common
{
    public class AsyncLoadScene : MonoBehaviour
    {
        public static string LoadSceneName;
        public Slider LoadingSlider;
        public Text ProgressText;
        public Text LoadingText;

        private float _targetValue;
        private AsyncOperation _operation;
        // Use this for initialization
        void Start()
        {
            LoadingSlider.value = 0.0f;
            ProgressText.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            StartCoroutine(AsyncLoading());
            StartCoroutine(UpdateLoadingText());
        }

        // Update is called once per frame
        void Update()
        {
            _targetValue = _operation.progress;

            if (_operation.progress >= 0.9f)
            {
                //operation.progress的值最大为0.9
                _targetValue = 1.0f;
            }

            LoadingSlider.value = Mathf.Lerp(LoadingSlider.value, _targetValue, Time.deltaTime);
            if (_targetValue - LoadingSlider.value < 0.01f)
            {
                LoadingSlider.value = _targetValue;
            }

            ProgressText.text = ((int)(LoadingSlider.value * 100)).ToString() + "%";
            ProgressText.GetComponent<RectTransform>().anchoredPosition = new Vector2(LoadingSlider.value * (LoadingSlider.GetComponent<RectTransform>().sizeDelta.x - ProgressText.GetComponent<RectTransform>().sizeDelta.x), 0f);

            if ((int)(LoadingSlider.value * 100) == 100)
            {
                //允许异步加载完毕后自动切换场景
                _operation.allowSceneActivation = true;
            }
        }

        IEnumerator AsyncLoading()
        {
            _operation = SceneManager.LoadSceneAsync(LoadSceneName ?? throw new ArgumentNullException(LoadSceneName));
            //阻止当加载完成自动切换
            _operation.allowSceneActivation = false;

            yield return _operation;
        }

        IEnumerator UpdateLoadingText()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.3f);
                LoadingText.text = "Loading.";
                yield return new WaitForSeconds(0.3f);
                LoadingText.text = "Loading..";
                yield return new WaitForSeconds(0.3f);
                LoadingText.text = "Loading...";
                yield return new WaitForSeconds(0.3f);
                LoadingText.text = "Loading";
            }

        }
    }
}
