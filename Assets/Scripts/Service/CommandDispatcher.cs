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
        private RecvBufQueue _recvBufQueue;
        private AppFacade _appFacade;
        private Dictionary<string, Lazy<Action<JObject>>> _commandDict;

        public CommandDispatcher()
        {
            _recvBufQueue = NetworkService.Instance.RecvBufQueue;
            _appFacade = AppFacade.Instance;
            InitCommandDict();
        }

        public void StartDispatch(object state)
        {
            if (state is SynchronizationContext context)
            {
                while (true)
                {
                    _recvBufQueue.MessageComeEvent.Wait();
                    // 分发命令
                    var msg = _recvBufQueue.Dequeue();
                    if (msg != null)
                    {
                        JObject o = JObject.Parse(msg);
                        string command = (string)o.SelectToken("Command");
                        o.Property("Command").Remove();

                        if (_commandDict.TryGetValue(command, out var lazyAction))
                        {
                            //封送回主线程执行
                            context.Post((obj) => lazyAction.Value(obj as JObject), o);
                        }
                        else
                        {
                            Debug.Log("Command not found");
                        }
                    }
                }
            }
        }

        private void InitCommandDict() => _commandDict = new Dictionary<string, Lazy<Action<JObject>>>
            {
                { NotifyConsts.LoginNotification.LoginResult, new Lazy<Action<JObject>>(() => (_appFacade.RetrieveProxy(nameof(LoginProxy)) as LoginProxy).LoginResult) },
                { NotifyConsts.LoginNotification.LogoutResult, new Lazy<Action<JObject>>(() => (_appFacade.RetrieveProxy(nameof(LoginProxy)) as LoginProxy).LogoutResult) },
                { NotifyConsts.LoginNotification.RegisterResult, new Lazy<Action<JObject>>(() => (_appFacade.RetrieveProxy(nameof(LoginProxy)) as LoginProxy).RegisterResult) },

                { NotifyConsts.MainMenuNotification.ChangePasswordResult, new Lazy<Action<JObject>>(() => (_appFacade.RetrieveProxy(nameof(MainMenuProxy)) as MainMenuProxy).ChangePasswordResult) },

                { NotifyConsts.StoreNotification.StoreItemListResult, new Lazy<Action<JObject>>(() => (_appFacade.RetrieveProxy(nameof(StoreProxy)) as StoreProxy).StoreItemListResult) },
                { NotifyConsts.StoreNotification.PurchaseItemResult, new Lazy<Action<JObject>>(() => (_appFacade.RetrieveProxy(nameof(StoreProxy)) as StoreProxy).PurchaseItemResult) },

                { NotifyConsts.GarageNotification.ChangeVehicleResult, new Lazy<Action<JObject>>(() => (_appFacade.RetrieveProxy(nameof(GarageProxy)) as GarageProxy).ChangeVehicleResult) },

                { NotifyConsts.LobbyNotification.CreateRoomResult, new Lazy<Action<JObject>>(() => (_appFacade.RetrieveProxy(nameof(LobbyProxy)) as LobbyProxy).CreateRoomResult) },
                { NotifyConsts.LobbyNotification.JoinRoomResult, new Lazy<Action<JObject>>(() => (_appFacade.RetrieveProxy(nameof(LobbyProxy)) as LobbyProxy).JoinRoomResult) },
                { NotifyConsts.LobbyNotification.SearchRoomResult, new Lazy<Action<JObject>>(() => (_appFacade.RetrieveProxy(nameof(LobbyProxy)) as LobbyProxy).SearchRoomResult) },
                { NotifyConsts.LobbyNotification.RefreshRoomListResult, new Lazy<Action<JObject>>(() => (_appFacade.RetrieveProxy(nameof(LobbyProxy)) as LobbyProxy).RefreshRoomListResult) },

                { NotifyConsts.RoomNotification.ExitRoomResult, new Lazy<Action<JObject>>(() => (_appFacade.RetrieveProxy(nameof(RoomProxy)) as RoomProxy).ExitRoomResult) },
                { NotifyConsts.RoomNotification.ChangePrepareStateResult, new Lazy<Action<JObject>>(() => (_appFacade.RetrieveProxy(nameof(RoomProxy)) as RoomProxy).ChangePrepareStateResult) },
                { NotifyConsts.RoomNotification.KickPlayerResult, new Lazy<Action<JObject>>(() => (_appFacade.RetrieveProxy(nameof(RoomProxy)) as RoomProxy).KickPlayerResult) },
                { NotifyConsts.RoomNotification.SendMessageResult, new Lazy<Action<JObject>>(() => (_appFacade.RetrieveProxy(nameof(RoomProxy)) as RoomProxy).SendMessageResult) },
                { NotifyConsts.RoomNotification.StartGameResult, new Lazy<Action<JObject>>(() => (_appFacade.RetrieveProxy(nameof(RoomProxy)) as RoomProxy).StartGameResult) }
            };
    }
}
