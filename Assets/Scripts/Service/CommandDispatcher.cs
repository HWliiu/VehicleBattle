using GameClient.Model;
using GameClient.Service;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace GameClient.Service
{
    class CommandDispatcher
    {
        private static readonly Lazy<CommandDispatcher> _instance = new Lazy<CommandDispatcher>(() => new CommandDispatcher());
        public static CommandDispatcher Instance => _instance.Value;

        private RecvBufQueue _recvBufQueue;
        private WaitUntil _messageCome;

        public Dictionary<string, Action<JObject>> CommandDict { get; set; }

        private CommandDispatcher()
        {
            _recvBufQueue = NetworkService.Instance.RecvBufQueue;
            _messageCome = new WaitUntil(() => _recvBufQueue.MessageFlag);
            CommandDict = new Dictionary<string, Action<JObject>>();
        }

        public IEnumerator StartDispatch()
        {
            while (true)
            {
                yield return _messageCome;
                // 分发命令
                var msg = _recvBufQueue.Dequeue();
                if (msg != null)
                {
                    JObject o = JObject.Parse(msg);
                    string command = (string)o.SelectToken("Command");
                    o.Property("Command").Remove();
                    if (CommandDict.TryGetValue(command, out var action))
                    {
                        action(o);
                    }
                    else
                    {
                        Debug.Log($"{command} Command not found");
                    }
                }
            }
        }
    }
}
