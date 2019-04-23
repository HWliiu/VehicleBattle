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
        public RectTransform MainMenuPanel;
        public RectTransform BriefInfoPanel;
        public RectTransform PreviewPanel;
        public RectTransform VehicleParaPanel;
        public RectTransform UserInfoPanel;
        public RectTransform ChangePasswordPanel;
        public RectTransform StorePanel;
        public RectTransform GaragePanel;
        public RectTransform SettingPanel;
        public RectTransform DialogPanel;
        #region MainMenuPanel组件
        public Text MM_ConnStateText { get; set; }
        public Button MM_UserInfoBtn { get; set; }
        public Button MM_GarageBtn { get; set; }
        public Button MM_StoreBtn { get; set; }
        public Button MM_SettingBtn { get; set; }
        public Button MM_StartGameBtn { get; set; }
        #endregion
        #region BriefInfoPanel组件
        public Text BI_LevelText { get; set; }
        public Text BI_ExperienceText { get; set; }
        public Text BI_MoneyText { get; set; }
        #endregion
        #region VehicleParaPanel组件
        public Text VP_VehicleNameText { get; set; }
        public Slider[] VP_VehicleParaSliders { get; set; }
        #endregion
        #region UserInfoPanel组件
        public Text UI_UserIdText { get; set; }
        public Text UI_UserNameText { get; set; }
        public Text UI_UserExperienceText { get; set; }
        public Text UI_UserMoneyText { get; set; }
        public Text UI_UserCurVehicleText { get; set; }
        public Text UI_UserPossessVehicleText { get; set; }
        public Text UI_UserRegisterTimeText { get; set; }
        public Text UI_UserLoginTimeText { get; set; }
        public Button UI_ChangePasswordBtn { get; set; }
        public Button UI_LogoutBtn { get; set; }
        public Button UI_ExitGameBtn { get; set; }
        public Button UI_BackMainMenuBtn { get; set; }
        #endregion
        #region ChangePasswordPanel组件
        public InputField CP_OldPasswordInput { get; set; }
        public InputField CP_NewPasswordInput { get; set; }
        public InputField CP_ConfirmPasswordInput { get; set; }
        public Button CP_ConfirmChangeBtn { get; set; }
        public Button CP_ClearInputBtn { get; set; }
        public Button CP_BackUserInfoBtn { get; set; }
        public Text CP_ChangePasswordTipsText { get; set; }
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            InitComponent();
            AppFacade.Instance.RegisterMediator(new MainMenuMediator(nameof(MainMenuMediator), this));
            AppFacade.Instance.RegisterCommand(NotifyConsts.MainMenuNotification.RequestChangePassword, () => new ChangePwdCommand());
            AppFacade.Instance.RegisterProxy(new MainMenuProxy(nameof(MainMenuProxy), null));

            //UserInfoPanel相关
            MM_UserInfoBtn.onClick.AddListener(OnUserInfoBtn);
            UI_BackMainMenuBtn.onClick.AddListener(OnBackMainMenuBtn);
            UI_ExitGameBtn.onClick.AddListener(OnExitGameBtn);
            //ChangePasswordPanel相关
            UI_ChangePasswordBtn.onClick.AddListener(OnChangePasswordBtn);
            CP_ClearInputBtn.onClick.AddListener(OnClearInputBtn);
            CP_BackUserInfoBtn.onClick.AddListener(OnBackUserInfoBtn);
            //MainMenuPanel相关
            MM_GarageBtn.onClick.AddListener(OnGarageBtn);
            MM_StoreBtn.onClick.AddListener(OnStoreBtn);
            MM_SettingBtn.onClick.AddListener(OnSettingBtn);
            MM_StartGameBtn.onClick.AddListener(OnStartGameBtn);
        }

        void InitComponent()
        {
            //递归查找给定名字的物体组件,一定要保证子物体下没有重名的该物体,还要保证属性的名称与场景中的一致
            //初始化MainMenuPanel组件

            //初始化BriefInfoPanel组件
            BI_LevelText = UnityUtil.FindChild<Text>(BriefInfoPanel.transform, nameof(BI_LevelText)) ?? throw new ArgumentNullException(nameof(BI_LevelText));
            BI_ExperienceText = UnityUtil.FindChild<Text>(BriefInfoPanel.transform, nameof(BI_ExperienceText)) ?? throw new ArgumentNullException(nameof(BI_ExperienceText));
            BI_MoneyText = UnityUtil.FindChild<Text>(BriefInfoPanel.transform, nameof(BI_MoneyText)) ?? throw new ArgumentNullException(nameof(BI_MoneyText));
            //初始化PreviewPanel组件

            //初始化VehicleParaPanel组件
            VP_VehicleNameText = UnityUtil.FindChild<Text>(VehicleParaPanel.transform, nameof(VP_VehicleNameText)) ?? throw new ArgumentNullException(nameof(VP_VehicleNameText));
            VP_VehicleParaSliders = VehicleParaPanel.gameObject.GetComponentsInChildren<Slider>() ?? throw new ArgumentNullException(nameof(VP_VehicleParaSliders));
            //初始化UserInfoPanel组件
            UI_UserIdText = UnityUtil.FindChild<Text>(UserInfoPanel.transform, nameof(UI_UserIdText)) ?? throw new ArgumentNullException(nameof(UI_UserIdText));
            UI_UserNameText = UnityUtil.FindChild<Text>(UserInfoPanel.transform, nameof(UI_UserNameText)) ?? throw new ArgumentNullException(nameof(UI_UserNameText));
            UI_UserExperienceText = UnityUtil.FindChild<Text>(UserInfoPanel.transform, nameof(UI_UserExperienceText)) ?? throw new ArgumentNullException(nameof(UI_UserExperienceText));
            UI_UserMoneyText = UnityUtil.FindChild<Text>(UserInfoPanel.transform, nameof(UI_UserMoneyText)) ?? throw new ArgumentNullException(nameof(UI_UserMoneyText));
            UI_UserCurVehicleText = UnityUtil.FindChild<Text>(UserInfoPanel.transform, nameof(UI_UserCurVehicleText)) ?? throw new ArgumentNullException(nameof(UI_UserCurVehicleText));
            UI_UserPossessVehicleText = UnityUtil.FindChild<Text>(UserInfoPanel.transform, nameof(UI_UserPossessVehicleText)) ?? throw new ArgumentNullException(nameof(UI_UserPossessVehicleText));
            UI_UserRegisterTimeText = UnityUtil.FindChild<Text>(UserInfoPanel.transform, nameof(UI_UserRegisterTimeText)) ?? throw new ArgumentNullException(nameof(UI_UserRegisterTimeText));
            UI_UserLoginTimeText = UnityUtil.FindChild<Text>(UserInfoPanel.transform, nameof(UI_UserLoginTimeText)) ?? throw new ArgumentNullException(nameof(UI_UserLoginTimeText));
            UI_ChangePasswordBtn = UnityUtil.FindChild<Button>(UserInfoPanel.transform, nameof(UI_ChangePasswordBtn)) ?? throw new ArgumentNullException(nameof(UI_ChangePasswordBtn));
            UI_LogoutBtn = UnityUtil.FindChild<Button>(UserInfoPanel.transform, nameof(UI_LogoutBtn)) ?? throw new ArgumentNullException(nameof(UI_LogoutBtn));
            UI_ExitGameBtn = UnityUtil.FindChild<Button>(UserInfoPanel.transform, nameof(UI_ExitGameBtn)) ?? throw new ArgumentNullException(nameof(UI_ExitGameBtn));
            UI_BackMainMenuBtn = UnityUtil.FindChild<Button>(UserInfoPanel.transform, nameof(UI_BackMainMenuBtn)) ?? throw new ArgumentNullException(nameof(UI_BackMainMenuBtn));
            //初始化ChangePasswordPanel组件
            CP_OldPasswordInput = UnityUtil.FindChild<InputField>(ChangePasswordPanel.transform, nameof(CP_OldPasswordInput)) ?? throw new ArgumentNullException(nameof(CP_OldPasswordInput));
            CP_NewPasswordInput = UnityUtil.FindChild<InputField>(ChangePasswordPanel.transform, nameof(CP_NewPasswordInput)) ?? throw new ArgumentNullException(nameof(CP_NewPasswordInput));
            CP_ConfirmPasswordInput = UnityUtil.FindChild<InputField>(ChangePasswordPanel.transform, nameof(CP_ConfirmPasswordInput)) ?? throw new ArgumentNullException(nameof(CP_ConfirmPasswordInput));
            CP_ConfirmChangeBtn = UnityUtil.FindChild<Button>(ChangePasswordPanel.transform, nameof(CP_ConfirmChangeBtn)) ?? throw new ArgumentNullException(nameof(CP_ConfirmChangeBtn));
            CP_ClearInputBtn = UnityUtil.FindChild<Button>(ChangePasswordPanel.transform, nameof(CP_ClearInputBtn)) ?? throw new ArgumentNullException(nameof(CP_ClearInputBtn));
            CP_BackUserInfoBtn = UnityUtil.FindChild<Button>(ChangePasswordPanel.transform, nameof(CP_BackUserInfoBtn)) ?? throw new ArgumentNullException(nameof(CP_BackUserInfoBtn));
            CP_ChangePasswordTipsText = UnityUtil.FindChild<Text>(ChangePasswordPanel.transform, nameof(CP_ChangePasswordTipsText)) ?? throw new ArgumentNullException(nameof(CP_ChangePasswordTipsText));
            //初始化StorePanel组件
            //初始化GaragePanel组件
            //初始化SettingPanel组件
            //初始化DialogPanel组件
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

        private void OnUserInfoBtn() => UserInfoPanel.gameObject.SetActive(true);

        private void OnBackMainMenuBtn() => UserInfoPanel.gameObject.SetActive(false);

        private void OnExitGameBtn()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        private void OnChangePasswordBtn() => ChangePasswordPanel.gameObject.SetActive(true);
        private void OnClearInputBtn() => (CP_OldPasswordInput.text, CP_NewPasswordInput.text, CP_ConfirmPasswordInput.text, CP_ChangePasswordTipsText.text) = ("", "", "", "");
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
