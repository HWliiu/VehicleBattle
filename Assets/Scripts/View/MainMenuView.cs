using GameClient.Common;
using GameClient.Controller;
using GameClient.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient.View
{
    public class MainMenuView : MonoBehaviour
    {
        //只需要在编辑器里拖少数几个顶层对象即可,剩下的引用让代码去找
        public Text ConnStateText;
        public Button UserInfoBtn;
        public Button GarageBtn;
        public Button StoreBtn;
        public Button SettingBtn;
        public Button StartGameBtn;
        public RawImage UIMask;
        public RectTransform BriefInfoPanel;
        public RectTransform UserInfoPanel;
        public RectTransform VehicleParaPanel;
        public RectTransform ChangePasswordPanel;
        //BriefInfoPanel组件
        public Text LevelText { get; set; }
        public Text ExperienceText { get; set; }
        public Text MoneyText { get; set; }
        //VehicleParaPanel组件
        public Text VehicleNameText { get; set; }
        public Slider[] VehicleParaSliders { get; set; }
        //UserInfoPanel组件
        public Text UserIdText { get; set; }
        public Text UserNameText { get; set; }
        public Text UserExperienceText { get; set; }
        public Text UserMoneyText { get; set; }
        public Text UserCurVehicleText { get; set; }
        public Text UserPossessVehicleText { get; set; }
        public Text UserRegisterTimeText { get; set; }
        public Text UserLoginTimeText { get; set; }
        public Button ChangePasswordBtn { get; set; }
        public Button LogoutBtn { get; set; }
        public Button ExitGameBtn { get; set; }
        public Button BackMainMenuBtn { get; set; }
        //ChangePasswordPanel组件
        public InputField OldPasswordInput { get; set; }
        public InputField NewPasswordInput { get; set; }
        public InputField ConfirmPasswordInput { get; set; }
        public Button ConfirmChangeBtn { get; set; }
        public Button ClearInputBtn { get; set; }
        public Button BackUserInfoBtn { get; set; }
        public Text ChangePasswordTipsText { get; set; }

        // Start is called before the first frame update
        void Start()
        {
            InitComponent();
            AppFacade.Instance.RegisterMediator(new MainMenuMediator(nameof(MainMenuMediator), this));
            AppFacade.Instance.RegisterCommand(NotifyConsts.MainMenuNotification.RequestChangePassword, () => new ChangePwdCommand());
            AppFacade.Instance.RegisterProxy(new MainMenuProxy(nameof(MainMenuProxy), null));

            //UserInfoPanel相关
            UserInfoBtn.onClick.AddListener(OnUserInfoBtn);
            BackMainMenuBtn.onClick.AddListener(OnBackMainMenuBtn);
            ExitGameBtn.onClick.AddListener(OnExitGameBtn);
            //ChangePasswordPanel相关
            ChangePasswordBtn.onClick.AddListener(OnChangePasswordBtn);
            ClearInputBtn.onClick.AddListener(OnClearInputBtn);
            BackUserInfoBtn.onClick.AddListener(OnBackUserInfoBtn);
            //MainMenu相关
            GarageBtn.onClick.AddListener(OnGarageBtn);
            StoreBtn.onClick.AddListener(OnStoreBtn);
            SettingBtn.onClick.AddListener(OnSettingBtn);
            StartGameBtn.onClick.AddListener(OnStartGameBtn);
        }

        void InitComponent()
        {
            //初始化BriefInfoPanel组件
            LevelText = UnityUtil.FindChild<Text>(BriefInfoPanel.transform, nameof(LevelText)) ?? throw new ArgumentNullException(nameof(LevelText));   //递归查找给定名字的物体组件,一定要保证子物体下没有重名的该物体,还要保证属性的名称与场景中的一致
            ExperienceText = UnityUtil.FindChild<Text>(BriefInfoPanel.transform, nameof(ExperienceText)) ?? throw new ArgumentNullException(nameof(ExperienceText));
            MoneyText = UnityUtil.FindChild<Text>(BriefInfoPanel.transform, nameof(MoneyText)) ?? throw new ArgumentNullException(nameof(MoneyText));
            //初始化VehicleParaPanel组件
            VehicleNameText = UnityUtil.FindChild<Text>(VehicleParaPanel.transform, nameof(VehicleNameText)) ?? throw new ArgumentNullException(nameof(VehicleNameText));
            VehicleParaSliders = VehicleParaPanel.gameObject.GetComponentsInChildren<Slider>() ?? throw new ArgumentNullException(nameof(VehicleParaSliders));
            //初始化UserInfoPanel组件
            UserIdText = UnityUtil.FindChild<Text>(UserInfoPanel.transform, nameof(UserIdText)) ?? throw new ArgumentNullException(nameof(UserIdText));
            UserNameText = UnityUtil.FindChild<Text>(UserInfoPanel.transform, nameof(UserNameText)) ?? throw new ArgumentNullException(nameof(UserNameText));
            UserExperienceText = UnityUtil.FindChild<Text>(UserInfoPanel.transform, nameof(UserExperienceText)) ?? throw new ArgumentNullException(nameof(UserExperienceText));
            UserMoneyText = UnityUtil.FindChild<Text>(UserInfoPanel.transform, nameof(UserMoneyText)) ?? throw new ArgumentNullException(nameof(UserMoneyText));
            UserCurVehicleText = UnityUtil.FindChild<Text>(UserInfoPanel.transform, nameof(UserCurVehicleText)) ?? throw new ArgumentNullException(nameof(UserCurVehicleText));
            UserPossessVehicleText = UnityUtil.FindChild<Text>(UserInfoPanel.transform, nameof(UserPossessVehicleText)) ?? throw new ArgumentNullException(nameof(UserPossessVehicleText));
            UserRegisterTimeText = UnityUtil.FindChild<Text>(UserInfoPanel.transform, nameof(UserRegisterTimeText)) ?? throw new ArgumentNullException(nameof(UserRegisterTimeText));
            UserLoginTimeText = UnityUtil.FindChild<Text>(UserInfoPanel.transform, nameof(UserLoginTimeText)) ?? throw new ArgumentNullException(nameof(UserLoginTimeText));
            ChangePasswordBtn = UnityUtil.FindChild<Button>(UserInfoPanel.transform, nameof(ChangePasswordBtn)) ?? throw new ArgumentNullException(nameof(ChangePasswordBtn));
            LogoutBtn = UnityUtil.FindChild<Button>(UserInfoPanel.transform, nameof(LogoutBtn)) ?? throw new ArgumentNullException(nameof(LogoutBtn));
            ExitGameBtn = UnityUtil.FindChild<Button>(UserInfoPanel.transform, nameof(ExitGameBtn)) ?? throw new ArgumentNullException(nameof(ExitGameBtn));
            BackMainMenuBtn = UnityUtil.FindChild<Button>(UserInfoPanel.transform, nameof(BackMainMenuBtn)) ?? throw new ArgumentNullException(nameof(BackMainMenuBtn));
            //初始化ChangePasswordPanel组件
            OldPasswordInput = UnityUtil.FindChild<InputField>(ChangePasswordPanel.transform, nameof(OldPasswordInput)) ?? throw new ArgumentNullException(nameof(OldPasswordInput));
            NewPasswordInput = UnityUtil.FindChild<InputField>(ChangePasswordPanel.transform, nameof(NewPasswordInput)) ?? throw new ArgumentNullException(nameof(NewPasswordInput));
            ConfirmPasswordInput = UnityUtil.FindChild<InputField>(ChangePasswordPanel.transform, nameof(ConfirmPasswordInput)) ?? throw new ArgumentNullException(nameof(ConfirmPasswordInput));
            ConfirmChangeBtn = UnityUtil.FindChild<Button>(ChangePasswordPanel.transform, nameof(ConfirmChangeBtn)) ?? throw new ArgumentNullException(nameof(ConfirmChangeBtn));
            ClearInputBtn = UnityUtil.FindChild<Button>(ChangePasswordPanel.transform, nameof(ClearInputBtn)) ?? throw new ArgumentNullException(nameof(ClearInputBtn));
            BackUserInfoBtn = UnityUtil.FindChild<Button>(ChangePasswordPanel.transform, nameof(BackUserInfoBtn)) ?? throw new ArgumentNullException(nameof(BackUserInfoBtn));
            ChangePasswordTipsText = UnityUtil.FindChild<Text>(ChangePasswordPanel.transform, nameof(ChangePasswordTipsText)) ?? throw new ArgumentNullException(nameof(ChangePasswordTipsText));

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnDestroy()
        {
            AppFacade.Instance.RemoveMediator(nameof(MainMenuMediator));
            AppFacade.Instance.RemoveCommand(NotifyConsts.MainMenuNotification.RequestChangePassword);
            AppFacade.Instance.RemoveProxy(nameof(MainMenuProxy));

        }

        private void OnUserInfoBtn()
        {
            UserInfoPanel.gameObject.SetActive(true);
            UIMask.gameObject.SetActive(true);
        }

        private void OnBackMainMenuBtn()
        {
            UserInfoPanel.gameObject.SetActive(false);
            UIMask.gameObject.SetActive(false);
        }

        private void OnExitGameBtn()
        {
            //退出程序
        }
        private void OnChangePasswordBtn() => ChangePasswordPanel.gameObject.SetActive(true);
        private void OnClearInputBtn() => (OldPasswordInput.text, NewPasswordInput.text, ConfirmPasswordInput.text, ChangePasswordTipsText.text) = ("", "", "", "");
        private void OnBackUserInfoBtn()
        {
            OnClearInputBtn();
            ChangePasswordPanel.gameObject.SetActive(false);
        }

        private void OnGarageBtn()
        {

        }
        private void OnStoreBtn()
        {

        }
        private void OnSettingBtn()
        {

        }
        private void OnStartGameBtn()
        {

        }

    }
}
