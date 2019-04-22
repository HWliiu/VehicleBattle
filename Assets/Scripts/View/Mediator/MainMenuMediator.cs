using GameClient.Common;
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
    class MainMenuMediator : Mediator
    {
        private MainMenuView _viewComponent;
        public MainMenuMediator(string mediatorName, object viewComponent = null) : base(mediatorName, viewComponent)
        {
        }

        public override void HandleNotification(INotification notification)
        {
            switch (notification.Name)
            {
                case NotifyConsts.LoginNotification.LogoutResult:
                    break;
                case NotifyConsts.MainMenuNotification.ChangePasswordResult:
                    break;
                case NotifyConsts.MainMenuNotification.UpdateVehiclePara:  //预留扩展
                    break;
                case NotifyConsts.MainMenuNotification.UpdateUserInfo:  //预留扩展
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
                NotifyConsts.LoginNotification.LogoutResult,
                NotifyConsts.MainMenuNotification.ChangePasswordResult,
                NotifyConsts.MainMenuNotification.UpdateVehiclePara,
                NotifyConsts.MainMenuNotification.UpdateUserInfo,
                NotifyConsts.CommonNotification.UpdateConnState
            };
        }

        public override void OnRegister()
        {
            base.OnRegister();
            _viewComponent = (ViewComponent as MainMenuView) ?? throw new InvalidCastException(nameof(ViewComponent));

            UpdateUserInfo(PlayerManager.Instance.LocalPlayer);

            _viewComponent.OldPasswordInput.onEndEdit.AddListener(OnOldPasswordInputEndEdit);
            _viewComponent.NewPasswordInput.onEndEdit.AddListener(OnNewPasswordInputEndEdit);
            _viewComponent.ConfirmPasswordInput.onEndEdit.AddListener(OnConfirmPasswordInputEndEdit);
            _viewComponent.ConfirmChangeBtn.onClick.AddListener(OnConfirmChangeBtn);
            _viewComponent.LogoutBtn.onClick.AddListener(OnLogoutBtn);
        }

        public override void OnRemove()
        {
            base.OnRemove();
        }

        private void UpdateBriefInfo(string level, string experience, string money)
        {
            (_viewComponent.LevelText.text, _viewComponent.ExperienceText.text, _viewComponent.MoneyText.text) = (level, experience, money);
        }
        private void UpdateUserInfo(LocalPlayerVO localPlayerVO)
        {
            if (localPlayerVO == null)
            {
                Debug.Log("跳过登录");
                return;
            }

            _viewComponent.UserIdText.text = localPlayerVO.UserID;
            _viewComponent.UserNameText.text = localPlayerVO.UserName;
            _viewComponent.UserExperienceText.text = localPlayerVO.Experience.ToString("N0");
            _viewComponent.UserMoneyText.text = localPlayerVO.Money.ToString("C0");
            _viewComponent.UserCurVehicleText.text = localPlayerVO.CurVehicle.VehicleName;
            _viewComponent.UserPossessVehicleText.text = string.Join(", ", localPlayerVO.VehicleList.Select((vehicle) => vehicle.VehicleName));
            _viewComponent.UserRegisterTimeText.text = localPlayerVO.RegisterTime;
            _viewComponent.UserLoginTimeText.text = localPlayerVO.LoginTime;
            UpdateBriefInfo(localPlayerVO.Level.ToString(), localPlayerVO.Experience.ToString("N0"), localPlayerVO.Money.ToString("C0"));
            UpdateVehiclePara(localPlayerVO.CurVehicle.VehicleName, localPlayerVO.CurVehicle.Attack, localPlayerVO.CurVehicle.Motility, localPlayerVO.CurVehicle.Defend, localPlayerVO.CurVehicle.MaxHealth);
        }

        private void UpdateVehiclePara(string vehicleName, float vehicleAttack, float vehicleMotility, float vehicleDefend, int vehicleMaxHealth)
        {
            _viewComponent.VehicleNameText.text = vehicleName;
            _viewComponent.VehicleParaSliders[0].value = vehicleAttack;
            _viewComponent.VehicleParaSliders[0].GetComponentInChildren<Text>().text = vehicleAttack.ToString("F1");
            _viewComponent.VehicleParaSliders[1].value = vehicleMotility;
            _viewComponent.VehicleParaSliders[1].GetComponentInChildren<Text>().text = vehicleMotility.ToString("F1");
            _viewComponent.VehicleParaSliders[2].value = vehicleDefend;
            _viewComponent.VehicleParaSliders[2].GetComponentInChildren<Text>().text = vehicleDefend.ToString("F1");
            _viewComponent.VehicleParaSliders[3].value = vehicleMaxHealth;
            _viewComponent.VehicleParaSliders[3].GetComponentInChildren<Text>().text = vehicleMaxHealth.ToString();
        }

        private void OnOldPasswordInputEndEdit(string s)
        {
            if (!CheckPasswordFormat(s))
            {
                _viewComponent.ChangePasswordTipsText.text = "密码不合法";
            }
            else
            {
                _viewComponent.ChangePasswordTipsText.text = "";
            }
        }
        private void OnNewPasswordInputEndEdit(string s)
        {
            if (!CheckPasswordFormat(s))
            {
                _viewComponent.ChangePasswordTipsText.text = "密码不合法";
            }
            else
            {
                if (_viewComponent.OldPasswordInput.text == _viewComponent.NewPasswordInput.text)
                {
                    _viewComponent.ChangePasswordTipsText.text = "新密码不能与旧密码相同";
                }
                else
                {
                    _viewComponent.ChangePasswordTipsText.text = "";
                }
            }
        }
        private void OnConfirmPasswordInputEndEdit(string s)
        {
            string password = _viewComponent.NewPasswordInput.text;
            if (!CheckPasswordFormat(s))
            {
                _viewComponent.ChangePasswordTipsText.text = "密码不合法";
                return;
            }
            if (password != s)
            {
                _viewComponent.ChangePasswordTipsText.text = "两次输入密码不一致";
            }
            else
            {
                _viewComponent.ChangePasswordTipsText.text = "";
            }
        }
        private void OnConfirmChangeBtn()
        {
            string oldPassword = _viewComponent.OldPasswordInput.text;
            string newPassword = _viewComponent.NewPasswordInput.text;
            string confirmPassword = _viewComponent.ConfirmPasswordInput.text;
            if (CheckPasswordFormat(oldPassword) && CheckPasswordFormat(newPassword) && CheckPasswordFormat(confirmPassword))
            {
                if (oldPassword != newPassword)
                {
                    if (newPassword == confirmPassword)
                    {
                        _viewComponent.ChangePasswordTipsText.text = "";
                        SendNotification(NotifyConsts.MainMenuNotification.RequestChangePassword, Tuple.Create(oldPassword, confirmPassword), nameof(Tuple<string, string>));
                    }
                    else
                    {
                        _viewComponent.ChangePasswordTipsText.text = "两次输入密码不一致";
                        return;
                    }
                }
                else
                {
                    _viewComponent.ChangePasswordTipsText.text = "新密码不能与旧密码相同";
                    return;
                }
            }
            else
            {
                _viewComponent.ChangePasswordTipsText.text = "密码不合法";
                return;
            }
        }
        private void OnLogoutBtn()
        {

        }
        private bool CheckPasswordFormat(string password) => RegexMatch.PasswordMatch(password);
    }
}
