using GameClient.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient.View
{
    public class LoginView : MonoBehaviour
    {
        public Text ConnStateText;
        [Header("LoginPanel")]
        public GameObject LoginPanel;
        public InputField UsernameInput;
        public InputField PasswordInput;
        public Button LoginBtn;
        public Button RegisterBtn;
        public Text LoginTipsText;
        [Header("RegisterPanel")]
        public GameObject RegisterPanel;
        public InputField RegisterUsernameInput;
        public InputField RegisterPasswordInput;
        public InputField ConfirmPasswordInput;
        public Button ConfirmRegisterBtn;
        public Button ClearBtn;
        public Button BackLoginBtn;
        public Text RegisterTipsText;

        //ViewComponent只负责UI的绘制，而其他事情，包括事件的绑定统统交给Mediator来做
        private void Start()    //所有的Mediator以及非全局的Command,Proxy在Start()时注册,OnDestroy()时移除
        {
            AppFacade.Instance.RegisterMediator(new LoginMediator(nameof(LoginView), this));
            AppFacade.Instance.RegisterCommand(NotifyConsts.LoginNotification.RequestLogin, () => new LoginCommand());
            AppFacade.Instance.RegisterCommand(NotifyConsts.LoginNotification.RequestRegister, () => new RegisterCommand());

            RegisterBtn.onClick.AddListener(OnRegisterBtn);
            BackLoginBtn.onClick.AddListener(OnBackLoginBtn);
            ClearBtn.onClick.AddListener(OnClearBtn);
        }

        private void OnDestroy()
        {
            AppFacade.Instance.RemoveMediator(nameof(LoginView));
            AppFacade.Instance.RemoveCommand(NotifyConsts.LoginNotification.RequestLogin);
            AppFacade.Instance.RemoveCommand(NotifyConsts.LoginNotification.RequestRegister);

            RegisterBtn.onClick.RemoveAllListeners();   //这两步移不移除无所谓，反正都被销毁了
            BackLoginBtn.onClick.RemoveAllListeners();
        }
        //纯UI控制的逻辑写到这里
        private void OnRegisterBtn()
        {
            LoginPanel.SetActive(false);
            RegisterPanel.SetActive(true);
            ClearLoginPanel();
        }

        private void OnBackLoginBtn()
        {
            RegisterPanel.SetActive(false);
            LoginPanel.SetActive(true);
            ClearRegisterPanel();
        }

        private void OnClearBtn()
        {
            ClearRegisterPanel();
        }

        public void ClearRegisterPanel()
        {
            RegisterUsernameInput.text = "";
            RegisterPasswordInput.text = "";
            ConfirmPasswordInput.text = "";
            RegisterTipsText.text = "";
        }

        public void ClearLoginPanel()
        {
            UsernameInput.text = "";
            PasswordInput.text = "";
            LoginTipsText.text = "";
        }
    }
}
