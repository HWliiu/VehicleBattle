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
        public RectTransform MainMenuPanel;
        public RectTransform BriefInfoPanel;
        //public RectTransform PreviewPanel;
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
        #region StorePanel组件
        public CustomScrollRect ST_ScrollView { get; set; }
        public Button ST_BackMainMenuBtn { get; set; }
        public Button ST_BuyVehicleBtn { get; set; }
        public Button ST_LeftBtn { get; set; }
        public Button ST_RightBtn { get; set; }
        #endregion
        #region GaragePanel组件
        public CustomScrollRect GR_ScrollView { get; set; }
        public Button GR_BackMainMenuBtn { get; set; }
        public Button GR_ChangeVehicleBtn { get; set; }
        public Button GR_LeftBtn { get; set; }
        public Button GR_RightBtn { get; set; }
        #endregion
        #region SettingPanel组件
        public Button SE_BackMainMenuBtn { get; set; }
        #endregion
        #region DialogPanel组件
        public Text DL_DialogTitleText { get; set; }
        public Text DL_DialogTipsText { get; set; }
        public Button DL_DialogConfirmBtn { get; set; }
        public Button DL_DialogCancelBtn { get; set; }
        #endregion

        public VehicleItem SelectStoreItem { get; set; }
        public VehicleItem SelectGarageItem { get; set; }

        // Start is called before the first frame update
        void Start()
        {
            InitMainMenuPanel();
            InitBriefInfoPanel();
            InitVehicleParaPanel();
            InitUserInfoPanel();
            InitChangePasswordPanel();
            InitStorePanel();
            InitGaragePanel();
            InitSettingPanel();
            InitDialogPanel();

            AppFacade.Instance.RegisterCommand(NotifyConsts.MainMenuNotification.RequestChangePassword, () => new ChangePwdCommand());
            AppFacade.Instance.RegisterCommand(NotifyConsts.StoreNotification.RequestStoreItemList, () => new RequestStoreItemCommand());
            AppFacade.Instance.RegisterCommand(NotifyConsts.StoreNotification.RequestPurchaseItem, () => new PurchaseCommand());
            AppFacade.Instance.RegisterCommand(NotifyConsts.GarageNotification.RequestChangeVehicle, () => new ChangeVehicleCommand());
            AppFacade.Instance.RegisterProxy(new MainMenuProxy(nameof(MainMenuProxy), null));
            AppFacade.Instance.RegisterProxy(new StoreProxy(nameof(StoreProxy), null));
            AppFacade.Instance.RegisterProxy(new GarageProxy(nameof(GarageProxy), null));
            AppFacade.Instance.RegisterMediator(new MainMenuMediator(nameof(MainMenuMediator), this));
        }
        void InitMainMenuPanel()
        {
            MM_ConnStateText = UnityUtil.FindChild<Text>(MainMenuPanel.transform, nameof(MM_ConnStateText)) ?? throw new ArgumentNullException(nameof(MM_ConnStateText));
            MM_UserInfoBtn = UnityUtil.FindChild<Button>(MainMenuPanel.transform, nameof(MM_UserInfoBtn)) ?? throw new ArgumentNullException(nameof(MM_UserInfoBtn));
            MM_GarageBtn = UnityUtil.FindChild<Button>(MainMenuPanel.transform, nameof(MM_GarageBtn)) ?? throw new ArgumentNullException(nameof(MM_GarageBtn));
            MM_StoreBtn = UnityUtil.FindChild<Button>(MainMenuPanel.transform, nameof(MM_StoreBtn)) ?? throw new ArgumentNullException(nameof(MM_StoreBtn));
            MM_SettingBtn = UnityUtil.FindChild<Button>(MainMenuPanel.transform, nameof(MM_SettingBtn)) ?? throw new ArgumentNullException(nameof(MM_SettingBtn));
            MM_StartGameBtn = UnityUtil.FindChild<Button>(MainMenuPanel.transform, nameof(MM_StartGameBtn)) ?? throw new ArgumentNullException(nameof(MM_StartGameBtn));

            MM_UserInfoBtn.onClick.AddListener(OnUserInfoBtn);
            MM_GarageBtn.onClick.AddListener(OnGarageBtn);
            MM_StoreBtn.onClick.AddListener(OnStoreBtn);
            MM_SettingBtn.onClick.AddListener(OnSettingBtn);
            MM_StartGameBtn.onClick.AddListener(OnStartGameBtn);
        }
        void InitBriefInfoPanel()
        {
            BI_LevelText = UnityUtil.FindChild<Text>(BriefInfoPanel.transform, nameof(BI_LevelText)) ?? throw new ArgumentNullException(nameof(BI_LevelText));
            BI_ExperienceText = UnityUtil.FindChild<Text>(BriefInfoPanel.transform, nameof(BI_ExperienceText)) ?? throw new ArgumentNullException(nameof(BI_ExperienceText));
            BI_MoneyText = UnityUtil.FindChild<Text>(BriefInfoPanel.transform, nameof(BI_MoneyText)) ?? throw new ArgumentNullException(nameof(BI_MoneyText));
        }
        void InitVehicleParaPanel()
        {
            VP_VehicleNameText = UnityUtil.FindChild<Text>(VehicleParaPanel.transform, nameof(VP_VehicleNameText)) ?? throw new ArgumentNullException(nameof(VP_VehicleNameText));
            VP_VehicleParaSliders = VehicleParaPanel.gameObject.GetComponentsInChildren<Slider>() ?? throw new ArgumentNullException(nameof(VP_VehicleParaSliders));
        }
        void InitUserInfoPanel()
        {
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

            UI_BackMainMenuBtn.onClick.AddListener(OnUserInfoBackMainMenuBtn);
            UI_ExitGameBtn.onClick.AddListener(OnExitGameBtn);
            UI_ChangePasswordBtn.onClick.AddListener(OnChangePasswordBtn);
        }
        void InitChangePasswordPanel()
        {
            CP_OldPasswordInput = UnityUtil.FindChild<InputField>(ChangePasswordPanel.transform, nameof(CP_OldPasswordInput)) ?? throw new ArgumentNullException(nameof(CP_OldPasswordInput));
            CP_NewPasswordInput = UnityUtil.FindChild<InputField>(ChangePasswordPanel.transform, nameof(CP_NewPasswordInput)) ?? throw new ArgumentNullException(nameof(CP_NewPasswordInput));
            CP_ConfirmPasswordInput = UnityUtil.FindChild<InputField>(ChangePasswordPanel.transform, nameof(CP_ConfirmPasswordInput)) ?? throw new ArgumentNullException(nameof(CP_ConfirmPasswordInput));
            CP_ConfirmChangeBtn = UnityUtil.FindChild<Button>(ChangePasswordPanel.transform, nameof(CP_ConfirmChangeBtn)) ?? throw new ArgumentNullException(nameof(CP_ConfirmChangeBtn));
            CP_ClearInputBtn = UnityUtil.FindChild<Button>(ChangePasswordPanel.transform, nameof(CP_ClearInputBtn)) ?? throw new ArgumentNullException(nameof(CP_ClearInputBtn));
            CP_BackUserInfoBtn = UnityUtil.FindChild<Button>(ChangePasswordPanel.transform, nameof(CP_BackUserInfoBtn)) ?? throw new ArgumentNullException(nameof(CP_BackUserInfoBtn));
            CP_ChangePasswordTipsText = UnityUtil.FindChild<Text>(ChangePasswordPanel.transform, nameof(CP_ChangePasswordTipsText)) ?? throw new ArgumentNullException(nameof(CP_ChangePasswordTipsText));

            CP_ClearInputBtn.onClick.AddListener(OnClearInputBtn);
            CP_BackUserInfoBtn.onClick.AddListener(OnBackUserInfoBtn);
        }
        void InitStorePanel()
        {
            ST_ScrollView = UnityUtil.FindChild<CustomScrollRect>(StorePanel.transform, nameof(ST_ScrollView)) ?? throw new ArgumentNullException(nameof(ST_ScrollView));
            ST_BackMainMenuBtn = UnityUtil.FindChild<Button>(StorePanel.transform, nameof(ST_BackMainMenuBtn)) ?? throw new ArgumentNullException(nameof(ST_BackMainMenuBtn));
            ST_BuyVehicleBtn = UnityUtil.FindChild<Button>(StorePanel.transform, nameof(ST_BuyVehicleBtn)) ?? throw new ArgumentNullException(nameof(ST_BuyVehicleBtn));
            ST_LeftBtn = UnityUtil.FindChild<Button>(StorePanel.transform, nameof(ST_LeftBtn)) ?? throw new ArgumentNullException(nameof(ST_LeftBtn));
            ST_RightBtn = UnityUtil.FindChild<Button>(StorePanel.transform, nameof(ST_RightBtn)) ?? throw new ArgumentNullException(nameof(ST_RightBtn));

            ST_BackMainMenuBtn.onClick.AddListener(OnStoreBackMainMenu);
            ST_LeftBtn.onClick.AddListener(OnStoreLeftBtn);
            ST_RightBtn.onClick.AddListener(OnStoreRightBtn);
            ST_ScrollView.onValueChanged.AddListener(OnStoreScrollViewValueChanged);
        }
        void InitGaragePanel()
        {
            GR_ScrollView = UnityUtil.FindChild<CustomScrollRect>(GaragePanel.transform, nameof(GR_ScrollView)) ?? throw new ArgumentNullException(nameof(GR_ScrollView));
            GR_BackMainMenuBtn = UnityUtil.FindChild<Button>(GaragePanel.transform, nameof(GR_BackMainMenuBtn)) ?? throw new ArgumentNullException(nameof(GR_BackMainMenuBtn));
            GR_ChangeVehicleBtn = UnityUtil.FindChild<Button>(GaragePanel.transform, nameof(GR_ChangeVehicleBtn)) ?? throw new ArgumentNullException(nameof(GR_ChangeVehicleBtn));
            GR_LeftBtn = UnityUtil.FindChild<Button>(GaragePanel.transform, nameof(GR_LeftBtn)) ?? throw new ArgumentNullException(nameof(GR_LeftBtn));
            GR_RightBtn = UnityUtil.FindChild<Button>(GaragePanel.transform, nameof(GR_RightBtn)) ?? throw new ArgumentNullException(nameof(GR_RightBtn));

            GR_BackMainMenuBtn.onClick.AddListener(OnGarageBackMainMenu);
            GR_LeftBtn.onClick.AddListener(OnGarageLeftBtn);
            GR_RightBtn.onClick.AddListener(OnGarageRightBtn);
            GR_ScrollView.onValueChanged.AddListener(OnGaragePanelScrollViewValueChanged);
        }
        void InitSettingPanel()
        {
            SE_BackMainMenuBtn = UnityUtil.FindChild<Button>(SettingPanel.transform, nameof(SE_BackMainMenuBtn)) ?? throw new ArgumentNullException(nameof(SE_BackMainMenuBtn));

            SE_BackMainMenuBtn.onClick.AddListener(OnSettingBackMainMenu);
        }
        void InitDialogPanel()
        {
            DL_DialogTitleText = UnityUtil.FindChild<Text>(DialogPanel.transform, nameof(DL_DialogTitleText)) ?? throw new ArgumentNullException(nameof(DL_DialogTitleText));
            DL_DialogTipsText = UnityUtil.FindChild<Text>(DialogPanel.transform, nameof(DL_DialogTipsText)) ?? throw new ArgumentNullException(nameof(DL_DialogTipsText));
            DL_DialogConfirmBtn = UnityUtil.FindChild<Button>(DialogPanel.transform, nameof(DL_DialogConfirmBtn)) ?? throw new ArgumentNullException(nameof(DL_DialogConfirmBtn));
            DL_DialogCancelBtn = UnityUtil.FindChild<Button>(DialogPanel.transform, nameof(DL_DialogCancelBtn)) ?? throw new ArgumentNullException(nameof(DL_DialogCancelBtn));

            DL_DialogCancelBtn.onClick.AddListener(OnDialogCancelBtn);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnDestroy()
        {
            AppFacade.Instance.RemoveCommand(NotifyConsts.MainMenuNotification.RequestChangePassword);
            AppFacade.Instance.RemoveCommand(NotifyConsts.StoreNotification.RequestStoreItemList);
            AppFacade.Instance.RemoveCommand(NotifyConsts.StoreNotification.RequestPurchaseItem);
            AppFacade.Instance.RemoveCommand(NotifyConsts.GarageNotification.RequestChangeVehicle);
            AppFacade.Instance.RemoveProxy(nameof(MainMenuProxy));
            AppFacade.Instance.RemoveProxy(nameof(StoreProxy));
            AppFacade.Instance.RemoveProxy(nameof(GarageProxy));
            AppFacade.Instance.RemoveMediator(nameof(MainMenuMediator));

        }
        #region MainMenuPanel
        private void OnUserInfoBtn() => UserInfoPanel.gameObject.SetActive(true);
        private void OnGarageBtn() => GaragePanel.gameObject.SetActive(true);
        private void OnStoreBtn() => StorePanel.gameObject.SetActive(true);
        private void OnSettingBtn() => SettingPanel.gameObject.SetActive(true);
        private void OnStartGameBtn() => UnityUtil.LoadScene(NotifyConsts.SceneName.RoomScene);
        #endregion
        #region UserInfoPanel
        private void OnUserInfoBackMainMenuBtn() => UserInfoPanel.gameObject.SetActive(false);
        private void OnExitGameBtn()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        private void OnChangePasswordBtn() => ChangePasswordPanel.gameObject.SetActive(true);
        #endregion
        #region ChangePasswordPanel
        private void OnClearInputBtn() => (CP_OldPasswordInput.text, CP_NewPasswordInput.text, CP_ConfirmPasswordInput.text, CP_ChangePasswordTipsText.text) = ("", "", "", "");
        private void OnBackUserInfoBtn()
        {
            OnClearInputBtn();
            ChangePasswordPanel.gameObject.SetActive(false);
        }
        #endregion
        #region StorePanel
        private void OnStoreBackMainMenu() => StorePanel.gameObject.SetActive(false);
        private void OnStoreLeftBtn()
        {
            ST_ScrollView.SetContentAnchoredPosition(new Vector2(ST_ScrollView.content.anchoredPosition.x + ST_ScrollView.content.GetComponent<GridLayoutGroup>().cellSize.x + ST_ScrollView.content.GetComponent<GridLayoutGroup>().padding.left, 0f));
        }
        private void OnStoreRightBtn()
        {
            ST_ScrollView.SetContentAnchoredPosition(new Vector2(ST_ScrollView.content.anchoredPosition.x - ST_ScrollView.content.GetComponent<GridLayoutGroup>().cellSize.x - ST_ScrollView.content.GetComponent<GridLayoutGroup>().padding.left, 0f));
        }
        private void OnStoreScrollViewValueChanged(Vector2 position)
        {
            if (position.x < 0.01f)
            {
                ST_ScrollView.SetContentAnchoredPosition(new Vector2(0f, 0f));
                ST_LeftBtn.interactable = false;
            }
            else if (position.x > 0.99f)
            {
                ST_ScrollView.SetContentAnchoredPosition(new Vector2(-ST_ScrollView.content.sizeDelta.x, 0f));
                ST_RightBtn.interactable = false;
            }
            else
            {
                if (ST_LeftBtn.interactable == false || ST_RightBtn.interactable == false)
                {
                    ST_LeftBtn.interactable = true;
                    ST_RightBtn.interactable = true;
                }
            }
        }
        public void InitStoreItemList(List<VehicleVO> vehicleList)
        {
            var storeItemPreb = Resources.Load<GameObject>("Prefab/StoreItem");
            for (int i = 0; i < vehicleList.Count; i++)
            {
                var item = AddItemToScrollView(vehicleList[i], storeItemPreb, ST_ScrollView);
                var toggle = item.GetComponent<Toggle>();
                toggle.onValueChanged.AddListener(isOn => { if (isOn) SelectStoreItem = item.GetComponent<VehicleItem>(); });
                if (i == 0) toggle.isOn = true;
            }
        }
        #endregion
        #region GaragePanel
        private void OnGarageBackMainMenu() => GaragePanel.gameObject.SetActive(false);
        private void OnGarageLeftBtn()
        {
            GR_ScrollView.SetContentAnchoredPosition(new Vector2(GR_ScrollView.content.anchoredPosition.x + GR_ScrollView.content.GetComponent<GridLayoutGroup>().cellSize.x + GR_ScrollView.content.GetComponent<GridLayoutGroup>().padding.left, 0f));
        }
        private void OnGarageRightBtn()
        {
            GR_ScrollView.SetContentAnchoredPosition(new Vector2(GR_ScrollView.content.anchoredPosition.x - GR_ScrollView.content.GetComponent<GridLayoutGroup>().cellSize.x - GR_ScrollView.content.GetComponent<GridLayoutGroup>().padding.left, 0f));
        }
        private void OnGaragePanelScrollViewValueChanged(Vector2 position)
        {
            if (position.x < 0.01f)
            {
                GR_ScrollView.SetContentAnchoredPosition(new Vector2(0f, 0f));
                GR_LeftBtn.interactable = false;
            }
            else if (position.x > 0.99f)
            {
                GR_ScrollView.SetContentAnchoredPosition(new Vector2(-GR_ScrollView.content.sizeDelta.x, 0f));
                GR_RightBtn.interactable = false;
            }
            else
            {
                if (GR_LeftBtn.interactable == false || GR_RightBtn.interactable == false)
                {
                    GR_LeftBtn.interactable = true;
                    GR_RightBtn.interactable = true;
                }
            }
        }
        public void InitGarageItemList(List<VehicleVO> vehicleList)
        {
            var garageItemPreb = Resources.Load<GameObject>("Prefab/GarageItem");
            foreach (var vehicle in vehicleList)
            {
                var item = AddItemToScrollView(vehicle, garageItemPreb, GR_ScrollView);
                var toggle = item.GetComponent<Toggle>();
                toggle.onValueChanged.AddListener(isOn => { if (isOn) SelectGarageItem = item.GetComponent<VehicleItem>(); });
                if (vehicle.VehicleID == PlayerManager.Instance.LocalPlayer.CurVehicle.VehicleID) toggle.isOn = true;
            }
        }
        public void AddGarageItem(VehicleVO vehicle)
        {
            var garageItemPreb = Resources.Load<GameObject>("Prefab/GarageItem");
            var item = AddItemToScrollView(vehicle, garageItemPreb, GR_ScrollView);
            item.GetComponent<Toggle>().onValueChanged.AddListener(isOn => { if (isOn) SelectGarageItem = item.GetComponent<VehicleItem>(); });
        }
        #endregion
        #region SettingPanel
        private void OnSettingBackMainMenu() => SettingPanel.gameObject.SetActive(false);
        #endregion
        #region DialogPanel
        public void OnDialogCancelBtn()
        {
            DL_DialogTitleText.text = "";
            DL_DialogTipsText.text = "";
            DL_DialogConfirmBtn.onClick.RemoveAllListeners();
            DialogPanel.gameObject.SetActive(false);
        }
        #endregion
        #region Common
        private Transform FindEmptyGrid(ScrollRect scrollView)
        {
            var content = scrollView.content;
            for (int i = 0; i < content.transform.childCount; i++)
            {
                if (content.transform.GetChild(i).childCount == 0)
                {
                    return content.transform.GetChild(i).transform;
                }
            }
            return null;
        }
        private GameObject AddItemToScrollView(VehicleVO vehicle, GameObject prefab, CustomScrollRect scrollView)
        {
            var grid = FindEmptyGrid(scrollView);
            GameObject item = Instantiate(prefab, grid) as GameObject;
            var itemTrans = item.GetComponent<RectTransform>();
            var gridTrans = grid.GetComponent<RectTransform>();
            var toggle = item.GetComponent<Toggle>();
            itemTrans.sizeDelta = gridTrans.sizeDelta;
            itemTrans.anchoredPosition = new Vector2(gridTrans.sizeDelta.x / 2, -gridTrans.sizeDelta.y / 2);
            toggle.group = scrollView.content.GetComponent<ToggleGroup>();
            var vehicleItem = item.GetComponent<VehicleItem>();
            (vehicleItem.VehicleID, vehicleItem.VehicleName, vehicleItem.VehicleType, vehicleItem.Attack, vehicleItem.Motility, vehicleItem.Defend, vehicleItem.MaxHealth, vehicleItem.Price, vehicleItem.Intro) = vehicle;
            return item;
        }
        //private void ClearGrid(ScrollRect scrollView)
        //{
        //    var content = scrollView.content;
        //    for (int i = 0; i < content.transform.childCount; i++)
        //    {
        //        var grid = content.transform.GetChild(i);
        //        for (int j = 0; j < grid.childCount; j++)
        //        {
        //            DestroyImmediate(grid.GetChild(j));
        //        }
        //    }
        //}
        #endregion
    }
}
