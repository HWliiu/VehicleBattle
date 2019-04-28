using GameClient.Common;
using GameClient.Service;
using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace GameClient.View
{
    class LoginMediator : Mediator
    {
        //Mediator 添加事件监听，发送或接受Notification，改变组件状态等
        private LoginView _viewComponent;
        public LoginMediator(string mediatorName, object viewComponent = null) : base(mediatorName, viewComponent)
        {
        }

        public override void HandleNotification(INotification notification)
        {
            switch (notification.Name)
            {
                case NotifyConsts.LoginNotification.LoginResult:
                    if (notification.Body is Tuple<bool, string> loginInfoTuple)
                    {
                        HandleLoginResult(loginInfoTuple.Item1, loginInfoTuple.Item2);
                    }
                    break;
                case NotifyConsts.LoginNotification.RegisterResult:
                    if (notification.Body is Tuple<bool, string> registerInfoTuple)
                    {
                        HandleRegisterResult(registerInfoTuple.Item1, registerInfoTuple.Item2);
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
            //列举感兴趣的通知
            return new string[] {
                NotifyConsts.LoginNotification.LoginResult,
                NotifyConsts.LoginNotification.RegisterResult,
                NotifyConsts.CommonNotification.UpdateConnState
            };
        }

        public override void OnRegister()
        {
            base.OnRegister();
            _viewComponent = (ViewComponent as LoginView) ?? throw new InvalidCastException(nameof(ViewComponent));

            _viewComponent.LoginBtn.onClick.AddListener(OnLoginBtn);
            _viewComponent.ConfirmRegisterBtn.onClick.AddListener(OnConfirmRegisterBtn);
            //登录面板前端验证
            _viewComponent.UserNameInput.onEndEdit.AddListener(OnLoginUsernameEndEdit);
            _viewComponent.PasswordInput.onEndEdit.AddListener(OnLoginPasswordEndEdit);
            //注册面板前端验证
            _viewComponent.RegisterUserNameInput.onEndEdit.AddListener(OnRegisterUsernameEndEdit);
            _viewComponent.RegisterPasswordInput.onEndEdit.AddListener(OnRegisterPasswordEndEdit);
            _viewComponent.ConfirmPasswordInput.onEndEdit.AddListener(OnConfirmPasswordEndEdit);
        }

        public override void OnRemove()
        {
            base.OnRemove();
        }

        private void HandleLoginResult(bool result, string info)
        {
            _viewComponent.LoginTipsText.text = info;
            if (result)
            {
                async Task subsequentHandle()
                {
                    await Task.Delay(500);
                    UnityUtil.LoadScene(NotifyConsts.SceneName.MainMenuScene);
                }
                _ = subsequentHandle();
            }
        }

        private void HandleRegisterResult(bool result, string info)
        {
            _viewComponent.RegisterTipsText.text = info;
            if (result)
            {
                var username = _viewComponent.RegisterUserNameInput.text;
                var password = _viewComponent.ConfirmPasswordInput.text;
                async Task subsequentHandle()
                {
                    await Task.Delay(500);
                    //自动填入用户名密码
                    _viewComponent.OnBackLoginBtn();
                    _viewComponent.UserNameInput.text = username;
                    _viewComponent.PasswordInput.text = password;
                }
                _ = subsequentHandle();
            }
        }

        private void OnLoginBtn()
        {
            string username = _viewComponent.UserNameInput.text;
            string password = _viewComponent.PasswordInput.text;
            if (CheckUsernameFormat(username) && CheckPasswordFormat(password))
            {
                SendNotification(NotifyConsts.LoginNotification.RequestLogin, Tuple.Create(username, password), nameof(Tuple<string, string>));
            }
            else
            {
                _viewComponent.LoginTipsText.text = "用户名或密码不合法";
            }
        }

        private void OnConfirmRegisterBtn()
        {
            string username = _viewComponent.RegisterUserNameInput.text;
            string password = _viewComponent.RegisterPasswordInput.text;
            string confirmPassword = _viewComponent.ConfirmPasswordInput.text;
            if (CheckUsernameFormat(username) && CheckPasswordFormat(password) && CheckPasswordFormat(confirmPassword))
            {
                if (password != confirmPassword)
                {
                    _viewComponent.RegisterTipsText.text = "两次密码不一致";
                    return;
                }
                SendNotification(NotifyConsts.LoginNotification.RequestRegister, Tuple.Create(username, confirmPassword), nameof(Tuple<string, string>));
            }
            else
            {
                _viewComponent.RegisterTipsText.text = "用户名或密码不合法";
            }
        }

        private bool CheckUsernameFormat(string username) => RegexMatch.UserNameMatch(username);

        private bool CheckPasswordFormat(string password) => RegexMatch.PasswordMatch(password);

        private void OnLoginUsernameEndEdit(string username)
        {
            if (!CheckUsernameFormat(username))
            {
                _viewComponent.LoginTipsText.text = "用户名不合法";
            }
            else
            {
                _viewComponent.LoginTipsText.text = "";
            }
        }
        private void OnLoginPasswordEndEdit(string password)
        {
            if (!CheckPasswordFormat(password))
            {
                _viewComponent.LoginTipsText.text = "密码不合法";
            }
            else
            {
                _viewComponent.LoginTipsText.text = "";
            }
        }
        private void OnRegisterUsernameEndEdit(string username)
        {
            if (!CheckUsernameFormat(username))
            {
                _viewComponent.RegisterTipsText.text = "用户名不合法";
            }
            else
            {
                _viewComponent.RegisterTipsText.text = "";
            }
        }
        private void OnRegisterPasswordEndEdit(string password)
        {
            if (!CheckPasswordFormat(password))
            {
                _viewComponent.RegisterTipsText.text = "密码不合法";
            }
            else
            {
                _viewComponent.RegisterTipsText.text = "";
            }
        }
        private void OnConfirmPasswordEndEdit(string confirmPassword)
        {
            string password = _viewComponent.RegisterPasswordInput.text;
            if (!CheckPasswordFormat(confirmPassword))
            {
                _viewComponent.RegisterTipsText.text = "密码不合法";
                return;
            }
            if (password != confirmPassword)
            {
                _viewComponent.RegisterTipsText.text = "两次输入密码不一致";
            }
            else
            {
                _viewComponent.RegisterTipsText.text = "";
            }
        }
    }
}
