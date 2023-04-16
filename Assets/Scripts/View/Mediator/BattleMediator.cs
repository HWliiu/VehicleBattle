using GameClient.Common;
using GameClient.Controller;
using GameClient.Model;
using GameClient.Service;
using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient.View
{
    class BattleMediator : Mediator
    {
        private BattleView _viewComponent;
        private LocalPlayerVO _localPlayer;

        public BattleMediator(string mediatorName, object viewComponent = null) : base(mediatorName, viewComponent)
        {
        }

        public override void HandleNotification(INotification notification)
        {
            switch (notification.Name)
            {
                case NotifyConsts.BattleNotification.UpdateTransformState:
                    if (notification.Body is PlayerVO playerTransformState)
                    {
                        HandleUpdateTransformState(playerTransformState);
                    }
                    break;
                case NotifyConsts.BattleNotification.UpdateFireState:
                    if (notification.Body is PlayerVO playerFireState)
                    {
                        HandleUpdateFireState(playerFireState);
                    }
                    break;
                case NotifyConsts.BattleNotification.UpdateHealthState:
                    if (notification.Body is PlayerVO playerHealthState)
                    {
                        HandleUpdateHealthState(playerHealthState);
                    }
                    break;
                case NotifyConsts.BattleNotification.EndGame:
                    if (notification.Body is Tuple<int, int, int> endGameTuple)
                    {
                        HandleEndGame(endGameTuple.Item1, endGameTuple.Item2, endGameTuple.Item3);
                    }
                    break;
                case NotifyConsts.BattleNotification.PlayerExit:
                    if (notification.Body is Tuple<string, string> playerExitTuple)
                    {
                        HandlePlayerExit(playerExitTuple.Item1, playerExitTuple.Item2);
                    }
                    break;
                case NotifyConsts.CommonNotification.UpdateConnState:
                    if (notification.Body is ConnectState state)
                    {
                        UnityUtil.UpdateConnStateDisplay(state, _viewComponent.ConnStateText);
                    }
                    break;
                default:
                    break;
            }
        }

        public override string[] ListNotificationInterests()
        {
            return new string[] {
                NotifyConsts.BattleNotification.UpdateTransformState,
                NotifyConsts.BattleNotification.UpdateFireState,
                NotifyConsts.BattleNotification.UpdateHealthState,
                NotifyConsts.BattleNotification.EndGame,
                NotifyConsts.BattleNotification.PlayerExit,
                NotifyConsts.CommonNotification.UpdateConnState
            };
        }

        private void InitPlayers()
        {
            var tankPrefab = Resources.Load<GameObject>("Prefabs/Tank");
            var panzerPrefab = Resources.Load<GameObject>("Prefabs/Panzer");
            for (int i = 0; i < PlayerManager.Instance.PlayerOrder.Length; i++)
            {
                var player = PlayerManager.Instance.GetPlayer(PlayerManager.Instance.PlayerOrder[i]);
                GameObject vehicle = null;
                switch (player.CurVehicle.VehicleType)
                {
                    case VehicleType.Tank:
                        vehicle = UnityEngine.Object.Instantiate(tankPrefab, _viewComponent.SpawnPoints[i].position, _viewComponent.SpawnPoints[i].rotation);
                        break;
                    case VehicleType.Panzer:
                        vehicle = UnityEngine.Object.Instantiate(panzerPrefab, _viewComponent.SpawnPoints[i].position, _viewComponent.SpawnPoints[i].rotation);
                        break;
                    default:
                        break;
                }
                var vehicleController = vehicle.GetComponent<VehicleController>();
                if (player.UserID == _localPlayer.UserID)
                {
                    vehicleController.IsLocalPlayer = true;
                    vehicleController.OnMove = UpLoadTransformState;
                    vehicleController.OnFire = UpLoadFireState;
                    Camera.main.GetComponent<CameraController>().Target = vehicle.transform;
                    vehicle.tag = "Player";

                    _viewComponent.PI_PlayerNameText.text = _localPlayer.UserName;
                    _viewComponent.PI_HealthyBar.maxValue = _localPlayer.CurVehicle.MaxHealth;
                    _viewComponent.PI_HealthyBar.value = _localPlayer.CurVehicle.MaxHealth;
                }
                else
                {
                    vehicleController.IsLocalPlayer = false;
                    UnitySingleton<UnityUtil>.GetInstance().DelayExecute(0.1f, () =>
                    {
                        vehicleController.transform.Find("HealthCanvas").gameObject.SetActive(true);
                        var healthBar = vehicleController.transform.Find("HealthCanvas").GetComponentInChildren<Slider>();
                        healthBar.maxValue = player.CurVehicle.MaxHealth;
                        healthBar.value = healthBar.maxValue;
                    });
                }
                player.VehicleController = vehicleController;

                player.VehicleController.Attack = player.CurVehicle.Attack;
                player.VehicleController.MoveSpeed = player.CurVehicle.Motility * 2;
                player.VehicleController.TurnSpeed = player.CurVehicle.Motility * 30;
                player.VehicleController.AimSpeed = player.CurVehicle.Motility * 0.3f;

                player.Health = player.CurVehicle.MaxHealth;
            }
        }

        private void UpLoadTransformState(Vector3 vehiclePosition, Quaternion vehicleRotation, Quaternion turretRotation, float moveInputValue, float turnInputValue)
        {
            _localPlayer.CurVehicle.VehiclePosition = vehiclePosition;
            _localPlayer.CurVehicle.VehicleRotation = vehicleRotation;
            _localPlayer.CurVehicle.TurretRotation = turretRotation;
            _localPlayer.CurVehicle.MoveInputValue = moveInputValue;
            _localPlayer.CurVehicle.TurnInputValue = turnInputValue;
            SendNotification(NotifyConsts.BattleNotification.UpLoadTransformState, _localPlayer.CurVehicle, null);
        }
        private void UpLoadFireState(Vector3 fireTransformPosition, Quaternion fireTransformRotation, Vector3 fireForce)
        {
            _localPlayer.CurVehicle.FireTransformPosition = fireTransformPosition;
            _localPlayer.CurVehicle.FireTransformRotation = fireTransformRotation;
            _localPlayer.CurVehicle.FireForce = fireForce;
            SendNotification(NotifyConsts.BattleNotification.UpLoadFireState, _localPlayer.CurVehicle, null);
        }
        private void UpLoadHealthState() { }

        private void HandleUpdateTransformState(PlayerVO player)
        {
            player.VehicleController.VehiclePosition = player.CurVehicle.VehiclePosition;
            player.VehicleController.VehicleRotation = player.CurVehicle.VehicleRotation;
            player.VehicleController.TurretRotation = player.CurVehicle.TurretRotation;
            player.VehicleController.MoveInputValue = player.CurVehicle.MoveInputValue;
            player.VehicleController.TurnInputValue = player.CurVehicle.TurnInputValue;
        }
        private void HandleUpdateFireState(PlayerVO player) => player.VehicleController.NetPlayerFire(player.CurVehicle.FireTransformPosition, player.CurVehicle.FireTransformRotation, player.CurVehicle.FireForce);
        private void HandleUpdateHealthState(PlayerVO player)
        {
            if (player.UserID == _localPlayer.UserID)
            {
                SendNotification(NotifyConsts.BattleNotification.UpLoadHealthState, player.Health, null);
                _viewComponent.PI_HealthyBar.value = player.Health;
            }
            else
            {
                player.VehicleController.transform.Find("HealthCanvas").GetComponentInChildren<Slider>().value = player.Health;
            }
            if (player.Health < 0)
            {
                _viewComponent.AddMessageItem($"{player.UserName} 被击败");
                player.VehicleController.Explosion();
            }
        }
        private void HandleEndGame(int rank, int experience, int money)
        {
            _viewComponent.SP_RankText.text = $"第{rank}名";
            _viewComponent.SP_AttainExperienceText.text = $"获得经验{experience}";
            _viewComponent.SP_AttainMoneyText.text = $"获得金币{money}";
            _viewComponent.SP_BackLobbyBtn.onClick.AddListener(() => UnityUtil.LoadScene(NotifyConsts.SceneName.RoomScene));
            UnitySingleton<UnityUtil>.GetInstance().DelayExecute(2, () => _viewComponent.SettlementPanel.gameObject.SetActive(true));
        }

        private void HandlePlayerExit(string info, string playerId)
        {
            _viewComponent.AddMessageItem(info);
            UnityEngine.Object.Destroy(PlayerManager.Instance.GetPlayer(playerId).VehicleController.gameObject);
        }

        public override void OnRegister()
        {
            base.OnRegister();
            _viewComponent = (ViewComponent as BattleView) ?? throw new InvalidCastException(nameof(ViewComponent));
            _localPlayer = PlayerManager.Instance.LocalPlayer;

            AppFacade.Instance.RegisterCommand(NotifyConsts.BattleNotification.UpLoadTransformState, () => new UpLoadTransformStateCommand());
            AppFacade.Instance.RegisterCommand(NotifyConsts.BattleNotification.UpLoadFireState, () => new UpLoadFireStateCommand());
            AppFacade.Instance.RegisterCommand(NotifyConsts.BattleNotification.UpLoadHealthState, () => new UpLoadHealthStateCommand());

            AppFacade.Instance.RegisterProxy(new BattleProxy(nameof(BattleProxy), null));

            InitPlayers();
            _viewComponent.ESC_ContinueBtn.onClick.AddListener(() =>
            {
                _viewComponent.ESCPanel.gameObject.SetActive(false);
                _localPlayer.VehicleController.CanControll = true;
            });
            _viewComponent.ESC_ExitGameBtn.onClick.AddListener(() => Application.Quit());
            SendNotification(NotifyConsts.CommonNotification.UpdateConnState, NetworkService.Instance.ConnectState, nameof(ConnectState));
        }

        public override void OnRemove()
        {
            base.OnRemove();
            AppFacade.Instance.RemoveCommand(NotifyConsts.BattleNotification.UpLoadTransformState);
            AppFacade.Instance.RemoveCommand(NotifyConsts.BattleNotification.UpLoadFireState);
            AppFacade.Instance.RemoveCommand(NotifyConsts.BattleNotification.UpLoadHealthState);

            AppFacade.Instance.RemoveProxy(nameof(BattleProxy));
        }
    }
}
