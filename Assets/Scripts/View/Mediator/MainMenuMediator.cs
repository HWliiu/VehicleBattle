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
                    if (notification.Body is Tuple<bool, string> logoutInfoTuple)
                    {
                        HandleLogoutResult(logoutInfoTuple.Item1, logoutInfoTuple.Item2);
                    }
                    break;
                case NotifyConsts.MainMenuNotification.ChangePasswordResult:
                    if (notification.Body is Tuple<bool, string> changePwdInfoTuple)
                    {
                        HandleChangePwdResult(changePwdInfoTuple.Item1, changePwdInfoTuple.Item2);
                    }
                    break;
                case NotifyConsts.MainMenuNotification.UpdateUserInfo:
                    if (notification.Body is LocalPlayerVO localPlayer)
                    {
                        UpdateUserInfo(localPlayer);
                    }
                    break;
                case NotifyConsts.StoreNotification.StoreItemListResult:
                    if (notification.Body is List<VehicleVO> vehicleList)
                    {
                        HandleStoreItemListResult(vehicleList);
                    }
                    break;
                case NotifyConsts.StoreNotification.PurchaseItemResult:
                    if (notification.Body is Tuple<bool, string, VehicleVO> purchaseInfoTuple)
                    {
                        HandlePurchaseItemResult(purchaseInfoTuple.Item1, purchaseInfoTuple.Item2, purchaseInfoTuple.Item3);
                    }
                    break;
                case NotifyConsts.GarageNotification.ChangeVehicleResult:
                    if (notification.Body is Tuple<bool, string, string> changeInfoTuple)
                    {
                        HandleChangeVehicleResult(changeInfoTuple.Item1, changeInfoTuple.Item2, changeInfoTuple.Item3);
                    }
                    break;
                case NotifyConsts.CommonNotification.UpdateConnState:
                    if (notification.Body is ConnectState state)
                    {
                        UnityUtil.UpdateConnStateDisplay(state, _viewComponent.MM_ConnStateText);
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
                NotifyConsts.MainMenuNotification.UpdateUserInfo,
                NotifyConsts.StoreNotification.StoreItemListResult,
                NotifyConsts.StoreNotification.PurchaseItemResult,
                NotifyConsts.GarageNotification.ChangeVehicleResult,
                NotifyConsts.CommonNotification.UpdateConnState
            };
        }

        public override void OnRegister()
        {
            base.OnRegister();
            _viewComponent = (ViewComponent as MainMenuView) ?? throw new InvalidCastException(nameof(ViewComponent));
            //ChangePasswordPanel
            _viewComponent.CP_OldPasswordInput.onEndEdit.AddListener(OnOldPasswordInputEndEdit);
            _viewComponent.CP_NewPasswordInput.onEndEdit.AddListener(OnNewPasswordInputEndEdit);
            _viewComponent.CP_ConfirmPasswordInput.onEndEdit.AddListener(OnConfirmPasswordInputEndEdit);
            _viewComponent.CP_ConfirmChangeBtn.onClick.AddListener(OnConfirmChangeBtn);
            //UserInfoPanel
            _viewComponent.UI_LogoutBtn.onClick.AddListener(OnLogoutBtn);
            //StorePanel
            _viewComponent.ST_BuyVehicleBtn.onClick.AddListener(OnBuyVehicleBtn);
            //GaragePanel
            _viewComponent.GR_ChangeVehicleBtn.onClick.AddListener(OnChangeVehicleBtn);
            //Init
            _viewComponent.InitGarageItemList(PlayerManager.Instance.LocalPlayer.VehicleList);

            SendNotification(NotifyConsts.MainMenuNotification.UpdateUserInfo, PlayerManager.Instance.LocalPlayer, nameof(LocalPlayerVO));
            SendNotification(NotifyConsts.StoreNotification.RequestStoreItemList, null, null);
            SendNotification(NotifyConsts.CommonNotification.UpdateConnState, NetworkService.Instance.ConnectState, nameof(ConnectState));
        }

        public override void OnRemove()
        {
            base.OnRemove();
        }

        #region MainMenuPanel
        private void UpdateBriefInfo(string level, string experience, string money)
        {
            (_viewComponent.BI_LevelText.text, _viewComponent.BI_ExperienceText.text, _viewComponent.BI_MoneyText.text) = (level, experience, money);
        }
        private void UpdateVehiclePara(string vehicleName, float vehicleAttack, float vehicleMotility, float vehicleDefend, int vehicleMaxHealth)
        {
            _viewComponent.VP_VehicleNameText.text = vehicleName;
            _viewComponent.VP_VehicleParaSliders[0].value = vehicleAttack;
            _viewComponent.VP_VehicleParaSliders[0].GetComponentInChildren<Text>().text = vehicleAttack.ToString("F1");
            _viewComponent.VP_VehicleParaSliders[1].value = vehicleMotility;
            _viewComponent.VP_VehicleParaSliders[1].GetComponentInChildren<Text>().text = vehicleMotility.ToString("F1");
            _viewComponent.VP_VehicleParaSliders[2].value = vehicleDefend;
            _viewComponent.VP_VehicleParaSliders[2].GetComponentInChildren<Text>().text = vehicleDefend.ToString("F1");
            _viewComponent.VP_VehicleParaSliders[3].value = vehicleMaxHealth;
            _viewComponent.VP_VehicleParaSliders[3].GetComponentInChildren<Text>().text = vehicleMaxHealth.ToString();
        }
        private void UpdateUserInfo(LocalPlayerVO localPlayerVO)
        {
            if (localPlayerVO == null)
            {
                Debug.Log("跳过了登录");
                return;
            }

            _viewComponent.UI_UserIdText.text = localPlayerVO.UserID;
            _viewComponent.UI_UserNameText.text = localPlayerVO.UserName;
            _viewComponent.UI_UserExperienceText.text = localPlayerVO.Experience.ToString("N0");
            _viewComponent.UI_UserMoneyText.text = localPlayerVO.Money.ToString("C0");
            _viewComponent.UI_UserCurVehicleText.text = localPlayerVO.CurVehicle.VehicleName;
            _viewComponent.UI_UserPossessVehicleText.text = string.Join(", ", localPlayerVO.VehicleList.Select((vehicle) => vehicle.VehicleName));
            _viewComponent.UI_UserRegisterTimeText.text = localPlayerVO.RegisterTime;
            _viewComponent.UI_UserLoginTimeText.text = localPlayerVO.LoginTime;
            UpdateBriefInfo(localPlayerVO.Level.ToString(), localPlayerVO.Experience.ToString("N0"), localPlayerVO.Money.ToString("C0"));
            UpdateVehiclePara(localPlayerVO.CurVehicle.VehicleName, localPlayerVO.CurVehicle.Attack, localPlayerVO.CurVehicle.Motility, localPlayerVO.CurVehicle.Defend, localPlayerVO.CurVehicle.MaxHealth);
        }
        #endregion
        #region ChangePasswordPanel
        private void OnOldPasswordInputEndEdit(string s)
        {
            if (!CheckPasswordFormat(s))
            {
                _viewComponent.CP_ChangePasswordTipsText.text = "密码不合法";
            }
            else
            {
                _viewComponent.CP_ChangePasswordTipsText.text = "";
            }
        }
        private void OnNewPasswordInputEndEdit(string s)
        {
            if (!CheckPasswordFormat(s))
            {
                _viewComponent.CP_ChangePasswordTipsText.text = "密码不合法";
            }
            else
            {
                if (_viewComponent.CP_OldPasswordInput.text == _viewComponent.CP_NewPasswordInput.text)
                {
                    _viewComponent.CP_ChangePasswordTipsText.text = "新密码不能与旧密码相同";
                }
                else
                {
                    _viewComponent.CP_ChangePasswordTipsText.text = "";
                }
            }
        }
        private void OnConfirmPasswordInputEndEdit(string s)
        {
            string password = _viewComponent.CP_NewPasswordInput.text;
            if (!CheckPasswordFormat(s))
            {
                _viewComponent.CP_ChangePasswordTipsText.text = "密码不合法";
                return;
            }
            if (password != s)
            {
                _viewComponent.CP_ChangePasswordTipsText.text = "两次输入密码不一致";
            }
            else
            {
                _viewComponent.CP_ChangePasswordTipsText.text = "";
            }
        }
        private void OnConfirmChangeBtn()
        {
            string oldPassword = _viewComponent.CP_OldPasswordInput.text;
            string newPassword = _viewComponent.CP_NewPasswordInput.text;
            string confirmPassword = _viewComponent.CP_ConfirmPasswordInput.text;
            if (CheckPasswordFormat(oldPassword) && CheckPasswordFormat(newPassword) && CheckPasswordFormat(confirmPassword))
            {
                if (oldPassword != newPassword)
                {
                    if (newPassword == confirmPassword)
                    {
                        _viewComponent.CP_ChangePasswordTipsText.text = "";
                        SendNotification(NotifyConsts.MainMenuNotification.RequestChangePassword, Tuple.Create(oldPassword, confirmPassword), nameof(Tuple<string, string>));
                    }
                    else
                    {
                        _viewComponent.CP_ChangePasswordTipsText.text = "两次输入密码不一致";
                        return;
                    }
                }
                else
                {
                    _viewComponent.CP_ChangePasswordTipsText.text = "新密码不能与旧密码相同";
                    return;
                }
            }
            else
            {
                _viewComponent.CP_ChangePasswordTipsText.text = "密码不合法";
                return;
            }
        }
        private bool CheckPasswordFormat(string password) => RegexMatch.PasswordMatch(password);
        private void HandleChangePwdResult(bool result, string info)
        {
            _viewComponent.CP_ChangePasswordTipsText.text = info;
            if (result)
            {
                async Task subsequentHandle()
                {
                    await Task.Delay(500);
                    _viewComponent.CP_ChangePasswordTipsText.text = "正在返回登录界面";
                    await Task.Delay(500);
                    UnityUtil.LoadScene(NotifyConsts.SceneName.LoginScene);
                }
                _ = subsequentHandle();
            }
        }
        #endregion
        #region UserInfoPanel
        private void OnLogoutBtn()
        {
            _viewComponent.DL_DialogTitleText.text = "是否确认注销登录?";
            _viewComponent.DL_DialogTipsText.text = "";
            _viewComponent.DL_DialogConfirmBtn.onClick.AddListener(OnConfirmLogoutBtn);
            _viewComponent.DialogPanel.gameObject.SetActive(true);
        }
        private void OnConfirmLogoutBtn() => SendNotification(NotifyConsts.LoginNotification.RequestLogout, null, null);
        private void HandleLogoutResult(bool result, string info)
        {
            _viewComponent.DL_DialogTipsText.text = info;
            if (result)
            {
                async Task subsequentHandle()
                {
                    await Task.Delay(500);
                    _viewComponent.DL_DialogTipsText.text = "正在返回登录界面";
                    await Task.Delay(500);
                    UnityUtil.LoadScene(NotifyConsts.SceneName.LoginScene);
                }
                _ = subsequentHandle();
            }
        }
        #endregion
        #region StorePanel
        private void HandleStoreItemListResult(List<VehicleVO> vehicleList) => _viewComponent.InitStoreItemList(vehicleList);
        private void OnBuyVehicleBtn()
        {
            _viewComponent.DL_DialogTitleText.text = "是否确认购买?";
            _viewComponent.DL_DialogTipsText.text = "";
            _viewComponent.DL_DialogConfirmBtn.onClick.AddListener(OnConfirmBuyVehicleBtn);
            _viewComponent.DialogPanel.gameObject.SetActive(true);
        }
        private void OnConfirmBuyVehicleBtn() => SendNotification(NotifyConsts.StoreNotification.RequestPurchaseItem, _viewComponent.SelectStoreItem.VehicleID, nameof(String));
        private void HandlePurchaseItemResult(bool result, string info, VehicleVO vehicle)
        {
            _viewComponent.DL_DialogTipsText.text = info;
            if (result)
            {
                _viewComponent.AddGarageItem(vehicle);
                async Task subsequentHandle()
                {
                    await Task.Delay(1000);
                    _viewComponent.OnDialogCancelBtn();
                }
                _ = subsequentHandle();
            }
        }
        #endregion
        #region GaragePanel
        private void OnChangeVehicleBtn()
        {
            _viewComponent.DL_DialogTitleText.text = "是否确认更换?";
            _viewComponent.DL_DialogTipsText.text = "";
            _viewComponent.DL_DialogConfirmBtn.onClick.AddListener(OnConfirmChangeVehicleBtn);
            _viewComponent.DialogPanel.gameObject.SetActive(true);
        }
        private void OnConfirmChangeVehicleBtn() => SendNotification(NotifyConsts.GarageNotification.RequestChangeVehicle, _viewComponent.SelectGarageItem.VehicleID, nameof(String));
        private void HandleChangeVehicleResult(bool result, string info, string vehicleId)
        {
            _viewComponent.DL_DialogTipsText.text = info;
            if (result)
            {
                _viewComponent.GR_ScrollView.content.GetComponentsInChildren<VehicleItem>().First((vehicleItem) => vehicleItem.VehicleID == vehicleId).GetComponent<Toggle>().isOn = true;
                async Task subsequentHandle()
                {
                    await Task.Delay(1000);
                    _viewComponent.OnDialogCancelBtn();
                }
                _ = subsequentHandle();
            }
        }
        #endregion
    }
}
