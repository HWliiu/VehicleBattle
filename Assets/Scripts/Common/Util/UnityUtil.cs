using GameClient.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameClient.Common
{
    public class UnityUtil : MonoBehaviour
    {
        public IEnumerator DelayExecute(float delayTime, Action action)
        {
            yield return new WaitForSeconds(delayTime);
            action();
        }
        public IEnumerator DelayExecute(float delayTime, Action<string> action, string state)
        {
            yield return new WaitForSeconds(delayTime);
            action(state);
        }
        public static void LoadScene(string sceneName)
        {
            AsyncLoadScene.LoadSceneName = sceneName;
            SceneManager.LoadScene(NotifyConsts.SceneName.LoadingScene);
        }
        public static void UpdateConnStateDisplay(ConnectState connectState, Text connectStateText)
        {
            switch (connectState)
            {
                case ConnectState.Disconnect:
                    connectStateText.text = "连接失败";
                    connectStateText.color = Color.red;
                    break;
                case ConnectState.Connecting:
                    connectStateText.text = "正在连接";
                    connectStateText.color = Color.yellow;
                    break;
                case ConnectState.Connected:
                    connectStateText.text = "已连接";
                    connectStateText.color = Color.green;
                    break;
                default:
                    break;
            }
        }
        public static Transform FindChild(Transform parent, string name)
        {
            Transform child = null;
            child = parent.Find(name);
            if (child != null)
                return child;
            Transform grandchild = null;
            for (int i = 0; i < parent.childCount; i++)
            {
                grandchild = FindChild(parent.GetChild(i), name);
                if (grandchild != null)
                    return grandchild;
            }
            return null;
        }

        public static T FindChild<T>(Transform parent, string name) where T : Component
        {
            Transform child = null;
            child = FindChild(parent, name);
            if (child != null)
                return child.GetComponent<T>();
            return null;
        }
    }
}
