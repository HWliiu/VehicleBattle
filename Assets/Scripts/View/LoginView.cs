using GameClient.Common;
using GameClient.Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameClient.View
{
    public class LoginView : MonoBehaviour
    {
        public Text ConnStateText;
        public RectTransform LoginPanel;
        public RectTransform RegisterPanel;

        #region LoginPanel组件
        public InputField UserNameInput { get; set; }
        public InputField PasswordInput { get; set; }
        public Button LoginBtn { get; set; }
        public Button RegisterBtn { get; set; }
        public Text LoginTipsText { get; set; }
        #endregion
        #region RegisterPanel组件
        public InputField RegisterUserNameInput { get; set; }
        public InputField RegisterPasswordInput { get; set; }
        public InputField ConfirmPasswordInput { get; set; }
        public Button ConfirmRegisterBtn { get; set; }
        public Button ClearInputBtn { get; set; }
        public Button BackLoginBtn { get; set; }
        public Text RegisterTipsText { get; set; }
        #endregion

        //ViewComponent只负责UI的绘制，而其他事情，包括事件的绑定统统交给Mediator来做
        private void Start()    //所有的Mediator以及非全局的Command,Proxy在Start()时注册,OnDestroy()时移除
        {
            Singleton<Main>.GetInstance();
            InitComponent();

            AppFacade.Instance.RegisterMediator(new LoginMediator(nameof(LoginView), this));
            AppFacade.Instance.RegisterCommand(NotifyConsts.LoginNotification.RequestLogin, () => new LoginCommand());
            AppFacade.Instance.RegisterCommand(NotifyConsts.LoginNotification.RequestRegister, () => new RegisterCommand());

            RegisterBtn.onClick.AddListener(OnRegisterBtn);
            BackLoginBtn.onClick.AddListener(OnBackLoginBtn);
            ClearInputBtn.onClick.AddListener(OnClearBtn);
        }
        void InitComponent()
        {
            //初始化LoginPanel组件
            UserNameInput = UnityUtil.FindChild<InputField>(LoginPanel.transform, nameof(UserNameInput)) ?? throw new ArgumentNullException(nameof(UserNameInput));
            PasswordInput = UnityUtil.FindChild<InputField>(LoginPanel.transform, nameof(PasswordInput)) ?? throw new ArgumentNullException(nameof(PasswordInput));
            LoginBtn = UnityUtil.FindChild<Button>(LoginPanel.transform, nameof(LoginBtn)) ?? throw new ArgumentNullException(nameof(LoginBtn));
            RegisterBtn = UnityUtil.FindChild<Button>(LoginPanel.transform, nameof(RegisterBtn)) ?? throw new ArgumentNullException(nameof(RegisterBtn));
            LoginTipsText = UnityUtil.FindChild<Text>(LoginPanel.transform, nameof(LoginTipsText)) ?? throw new ArgumentNullException(nameof(LoginTipsText));
            //初始化RegisterPanel组件
            RegisterUserNameInput = UnityUtil.FindChild<InputField>(RegisterPanel.transform, nameof(RegisterUserNameInput)) ?? throw new ArgumentNullException(nameof(RegisterUserNameInput));
            RegisterPasswordInput = UnityUtil.FindChild<InputField>(RegisterPanel.transform, nameof(RegisterPasswordInput)) ?? throw new ArgumentNullException(nameof(RegisterPasswordInput));
            ConfirmPasswordInput = UnityUtil.FindChild<InputField>(RegisterPanel.transform, nameof(ConfirmPasswordInput)) ?? throw new ArgumentNullException(nameof(ConfirmPasswordInput));
            ConfirmRegisterBtn = UnityUtil.FindChild<Button>(RegisterPanel.transform, nameof(ConfirmRegisterBtn)) ?? throw new ArgumentNullException(nameof(ConfirmRegisterBtn));
            ClearInputBtn = UnityUtil.FindChild<Button>(RegisterPanel.transform, nameof(ClearInputBtn)) ?? throw new ArgumentNullException(nameof(ClearInputBtn));
            BackLoginBtn = UnityUtil.FindChild<Button>(RegisterPanel.transform, nameof(BackLoginBtn)) ?? throw new ArgumentNullException(nameof(BackLoginBtn));
            RegisterTipsText = UnityUtil.FindChild<Text>(RegisterPanel.transform, nameof(RegisterTipsText)) ?? throw new ArgumentNullException(nameof(RegisterTipsText));
        }

        private void OnDestroy()
        {
            AppFacade.Instance.RemoveMediator(nameof(LoginView));
            AppFacade.Instance.RemoveCommand(NotifyConsts.LoginNotification.RequestLogin);
            AppFacade.Instance.RemoveCommand(NotifyConsts.LoginNotification.RequestRegister);
        }
        //纯UI控制的逻辑写到这里
        private void OnRegisterBtn()
        {
            LoginPanel.gameObject.SetActive(false);
            RegisterPanel.gameObject.SetActive(true);
            ClearLoginPanel();
        }

        public void OnBackLoginBtn()
        {
            RegisterPanel.gameObject.SetActive(false);
            LoginPanel.gameObject.SetActive(true);
            ClearRegisterPanel();
        }

        private void OnClearBtn()
        {
            ClearRegisterPanel();
        }

        public void ClearRegisterPanel() => (RegisterUserNameInput.text, RegisterPasswordInput.text, ConfirmPasswordInput.text, RegisterTipsText.text) = ("", "", "", "");

        public void ClearLoginPanel() => (UserNameInput.text, PasswordInput.text, LoginTipsText.text) = ("", "", "");
    }
}
