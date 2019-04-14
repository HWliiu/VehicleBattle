using GameClient;
using GameClient.Common;
using GameClient.Service;
using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace GameClient.View
{
    public class LoginMediator : Mediator
    {
        //Mediator 添加事件监听，发送或接受Notification，改变组件状态等
        private LoginView _loginViewComponent;
        public LoginMediator(string mediatorName, object viewComponent = null) : base(mediatorName, viewComponent)
        {
        }

        public override void HandleNotification(INotification notification)
        {
            string name = notification.Name;

            switch (name)
            {
                case NotifyConsts.LoginNotification.LoginResult:
                    // TODO: 处理登录结果
                    break;
                case NotifyConsts.LoginNotification.RegisterResult:
                    // TODO: 处理注册结果
                    break;
                case NotifyConsts.CommonNotification.UpdateConnState:
                    if (notification.Body is ConnectState state)
                    {
                        UpdateConnStateDisplay(state);
                    }
                    break;
                default:
                    break;
            }
        }

        public override string[] ListNotificationInterests()
        {
            //列举感兴趣的通知
            return new string[] { NotifyConsts.LoginNotification.LoginResult, NotifyConsts.LoginNotification.RegisterResult, NotifyConsts.CommonNotification.UpdateConnState };
        }

        public override void OnRegister()
        {
            base.OnRegister();
            _loginViewComponent = (ViewComponent as LoginView) ?? throw new Exception("ViewComponent cast error");

            _loginViewComponent.LoginBtn.onClick.AddListener(OnLoginBtn);
            _loginViewComponent.ConfirmRegisterBtn.onClick.AddListener(OnConfirmRegisterBtn);
            //登录面板前端验证
            _loginViewComponent.UsernameInput.onEndEdit.AddListener(OnLoginUsernameEndEdit);
            _loginViewComponent.PasswordInput.onEndEdit.AddListener(OnLoginPasswordEndEdit);
            //注册面板前端验证
            _loginViewComponent.RegisterUsernameInput.onEndEdit.AddListener(OnRegisterUsernameEndEdit);
            _loginViewComponent.RegisterPasswordInput.onEndEdit.AddListener(OnRegisterPasswordEndEdit);
            _loginViewComponent.ConfirmPasswordInput.onEndEdit.AddListener(OnConfirmPasswordEndEdit);
        }

        public override void OnRemove()
        {
            base.OnRemove();
        }

        private void UpdateConnStateDisplay(ConnectState connectState)
        {
            switch (connectState)
            {
                case ConnectState.Disconnect:
                    _loginViewComponent.ConnStateText.text = "连接失败";
                    break;
                case ConnectState.Connecting:
                    _loginViewComponent.ConnStateText.text = "正在连接";
                    break;
                case ConnectState.Connected:
                    _loginViewComponent.ConnStateText.text = "已连接";
                    break;
                default:
                    break;
            }
        }

        private void OnLoginBtn()
        {
            string username = _loginViewComponent.UsernameInput.text;
            string password = _loginViewComponent.PasswordInput.text;
            if (CheckUsernameFormat(username) && CheckPasswordFormat(password))
            {
                SendNotification(NotifyConsts.LoginNotification.RequestLogin, Tuple.Create(username, password), null);
            }
            else
            {
                _loginViewComponent.LoginTipsText.text = "用户名或密码不合法";
            }
        }

        private void OnConfirmRegisterBtn()
        {
            string username = _loginViewComponent.RegisterUsernameInput.text;
            string password = _loginViewComponent.RegisterPasswordInput.text;
            string confirmPassword = _loginViewComponent.ConfirmPasswordInput.text;
            if (CheckUsernameFormat(username) && CheckPasswordFormat(password) && CheckPasswordFormat(confirmPassword))
            {
                if (password != confirmPassword)
                {
                    _loginViewComponent.RegisterTipsText.text = "两次密码不一致";
                    return;
                }
                SendNotification(NotifyConsts.LoginNotification.RequestRegister, Tuple.Create(username, confirmPassword), null);
            }
            else
            {
                _loginViewComponent.RegisterTipsText.text = "用户名或密码不合法";
            }
        }

        private bool CheckUsernameFormat(string username)
        {
            return RegexMatch.UserNameMatch(username);
        }

        private bool CheckPasswordFormat(string password)
        {
            return RegexMatch.PasswordMatch(password);
        }

        private void OnLoginUsernameEndEdit(string username)
        {
            if (!CheckUsernameFormat(username))
            {
                _loginViewComponent.LoginTipsText.text = "用户名不合法";
            }
            else
            {
                _loginViewComponent.LoginTipsText.text = "";
            }
        }
        private void OnLoginPasswordEndEdit(string password)
        {
            if (!CheckPasswordFormat(password))
            {
                _loginViewComponent.LoginTipsText.text = "密码不合法";
            }
            else
            {
                _loginViewComponent.LoginTipsText.text = "";
            }
        }
        private void OnRegisterUsernameEndEdit(string username)
        {
            if (!CheckUsernameFormat(username))
            {
                _loginViewComponent.RegisterTipsText.text = "用户名不合法";
            }
            else
            {
                _loginViewComponent.RegisterTipsText.text = "";
            }
        }
        private void OnRegisterPasswordEndEdit(string password)
        {
            if (!CheckPasswordFormat(password))
            {
                _loginViewComponent.RegisterTipsText.text = "密码不合法";
            }
            else
            {
                _loginViewComponent.RegisterTipsText.text = "";
            }
        }
        private void OnConfirmPasswordEndEdit(string confirmPassword)
        {
            string password = _loginViewComponent.RegisterPasswordInput.text;
            if (!RegexMatch.PasswordMatch(confirmPassword))
            {
                _loginViewComponent.RegisterTipsText.text = "密码不合法";
                return;
            }
            if (password != confirmPassword)
            {
                _loginViewComponent.RegisterTipsText.text = "两次输入密码不一致";
            }
            else
            {
                _loginViewComponent.RegisterTipsText.text = "";
            }
        }
    }
}
