using GameClient.Common;
using GameClient.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient.View
{
    public class LobbyView : MonoBehaviour
    {
        public RectTransform LobbyPanel;
        public RectTransform RoomPanel;
        public RectTransform RoomListPanel;
        public RectTransform RoomInfoPanel;
        public RectTransform CreateRoomPanel;
        public RectTransform SearchRoomPanel;
        public RectTransform DialogPanel;
        public Text ConnStateText;
        #region LobbyPanel组件
        public Button LP_BackMainMenuBtn { get; set; }
        public Button LP_CreateRoomBtn { get; set; }
        public Button LP_SearchRoomBtn { get; set; }
        #endregion
        #region RoomListPanel组件
        public Button RL_RefreshRoomListBtn { get; set; }
        public ScrollRect RL_RoomListScrollView { get; set; }
        public Text RL_LobbyTipsText { get; set; }
        #endregion
        #region RoomInfoPanel组件
        public Text RI_RoomIDText { get; set; }
        public Text RI_RoomNameText { get; set; }
        public Text RI_OwnerNameText { get; set; }
        public Text RI_RoomModeText { get; set; }
        public Text RI_RoomMapText { get; set; }
        public Text RI_PlayerNumText { get; set; }
        #endregion
        #region CreateRoomPanel组件
        public Button CR_BackLobbyBtn { get; set; }
        public InputField CR_RoomNameInput { get; set; }
        public Toggle CR_SingleModeToggle { get; set; }
        public Toggle CR_TeamModeToggle { get; set; }
        public Dropdown CR_MapDropdown { get; set; }
        public Text CR_CreateRoomTipsText { get; set; }
        public Button CR_ConfirmCreateBtn { get; set; }
        #endregion
        #region SearchRoomPanel组件
        public Text SR_SearchRoomTipsText { get; set; }
        public Button SR_ConfirmSearchBtn { get; set; }
        public Button SR_BackLobbyBtn { get; set; }
        public InputField SR_SearchRoomInput { get; set; }
        #endregion
        #region DialogPanel组件
        public Text DL_DialogTitleText { get; set; }
        public Text DL_DialogTipsText { get; set; }
        public Button DL_DialogConfirmBtn { get; set; }
        public Button DL_DialogCancelBtn { get; set; }
        #endregion

        public RoomMode SelectRoomMode { get; set; }
        public RoomMap SelectRoomMap { get; set; }

        public event Action<string> OnJoinRoom;

        private GameObject _roomItemPrefab;

        // Start is called before the first frame update
        void Start()
        {
            InitLobbyPanel();
            InitRoomListPanel();
            InitRoomInfoPanel();
            InitCreateRoomPanel();
            InitSearchRoomPanel();
            InitDialogPanel();

            AppFacade.Instance.RegisterMediator(new LobbyMediator(nameof(LobbyMediator), this));

            _roomItemPrefab = Resources.Load<GameObject>("Prefab/RoomItem");
        }

        private void OnDestroy() => AppFacade.Instance.RemoveMediator(nameof(LobbyMediator));

        private void InitLobbyPanel()
        {
            LP_BackMainMenuBtn = UnityUtil.FindChild<Button>(LobbyPanel.transform, nameof(LP_BackMainMenuBtn)) ?? throw new ArgumentNullException(nameof(LP_BackMainMenuBtn));
            LP_CreateRoomBtn = UnityUtil.FindChild<Button>(LobbyPanel.transform, nameof(LP_CreateRoomBtn)) ?? throw new ArgumentNullException(nameof(LP_CreateRoomBtn));
            LP_SearchRoomBtn = UnityUtil.FindChild<Button>(LobbyPanel.transform, nameof(LP_SearchRoomBtn)) ?? throw new ArgumentNullException(nameof(LP_SearchRoomBtn));

            LP_BackMainMenuBtn.onClick.AddListener(() => UnityUtil.LoadScene(NotifyConsts.SceneName.MainMenuScene));
            LP_CreateRoomBtn.onClick.AddListener(() => CreateRoomPanel.gameObject.SetActive(true));
            LP_SearchRoomBtn.onClick.AddListener(() => SearchRoomPanel.gameObject.SetActive(true));
        }
        private void InitRoomListPanel()
        {
            RL_RefreshRoomListBtn = UnityUtil.FindChild<Button>(RoomListPanel.transform, nameof(RL_RefreshRoomListBtn)) ?? throw new ArgumentNullException(nameof(RL_RefreshRoomListBtn));
            RL_RoomListScrollView = UnityUtil.FindChild<ScrollRect>(RoomListPanel.transform, nameof(RL_RoomListScrollView)) ?? throw new ArgumentNullException(nameof(RL_RoomListScrollView));
            RL_LobbyTipsText = UnityUtil.FindChild<Text>(RoomListPanel.transform, nameof(RL_LobbyTipsText)) ?? throw new ArgumentNullException(nameof(RL_LobbyTipsText));
        }
        private void InitRoomInfoPanel()
        {
            RI_RoomIDText = UnityUtil.FindChild<Text>(RoomInfoPanel.transform, nameof(RI_RoomIDText)) ?? throw new ArgumentNullException(nameof(RI_RoomIDText));
            RI_RoomNameText = UnityUtil.FindChild<Text>(RoomInfoPanel.transform, nameof(RI_RoomNameText)) ?? throw new ArgumentNullException(nameof(RI_RoomNameText));
            RI_OwnerNameText = UnityUtil.FindChild<Text>(RoomInfoPanel.transform, nameof(RI_OwnerNameText)) ?? throw new ArgumentNullException(nameof(RI_OwnerNameText));
            RI_RoomModeText = UnityUtil.FindChild<Text>(RoomInfoPanel.transform, nameof(RI_RoomModeText)) ?? throw new ArgumentNullException(nameof(RI_RoomModeText));
            RI_RoomMapText = UnityUtil.FindChild<Text>(RoomInfoPanel.transform, nameof(RI_RoomMapText)) ?? throw new ArgumentNullException(nameof(RI_RoomMapText));
            RI_PlayerNumText = UnityUtil.FindChild<Text>(RoomInfoPanel.transform, nameof(RI_PlayerNumText)) ?? throw new ArgumentNullException(nameof(RI_PlayerNumText));
        }
        private void InitCreateRoomPanel()
        {
            CR_BackLobbyBtn = UnityUtil.FindChild<Button>(CreateRoomPanel.transform, nameof(CR_BackLobbyBtn)) ?? throw new ArgumentNullException(nameof(CR_BackLobbyBtn));
            CR_RoomNameInput = UnityUtil.FindChild<InputField>(CreateRoomPanel.transform, nameof(CR_RoomNameInput)) ?? throw new ArgumentNullException(nameof(CR_RoomNameInput));
            CR_SingleModeToggle = UnityUtil.FindChild<Toggle>(CreateRoomPanel.transform, nameof(CR_SingleModeToggle)) ?? throw new ArgumentNullException(nameof(CR_SingleModeToggle));
            CR_TeamModeToggle = UnityUtil.FindChild<Toggle>(CreateRoomPanel.transform, nameof(CR_TeamModeToggle)) ?? throw new ArgumentNullException(nameof(CR_TeamModeToggle));
            CR_MapDropdown = UnityUtil.FindChild<Dropdown>(CreateRoomPanel.transform, nameof(CR_MapDropdown)) ?? throw new ArgumentNullException(nameof(CR_MapDropdown));
            CR_CreateRoomTipsText = UnityUtil.FindChild<Text>(CreateRoomPanel.transform, nameof(CR_CreateRoomTipsText)) ?? throw new ArgumentNullException(nameof(CR_CreateRoomTipsText));
            CR_ConfirmCreateBtn = UnityUtil.FindChild<Button>(CreateRoomPanel.transform, nameof(CR_ConfirmCreateBtn)) ?? throw new ArgumentNullException(nameof(CR_ConfirmCreateBtn));

            CR_BackLobbyBtn.onClick.AddListener(OnCloseCreateRoomPanel);
            CR_SingleModeToggle.onValueChanged.AddListener(isOn => { if (isOn) SelectRoomMode = RoomMode.SingleMode; });
            CR_TeamModeToggle.onValueChanged.AddListener(isOn => { if (isOn) SelectRoomMode = RoomMode.TeamMode; });
            CR_MapDropdown.onValueChanged.AddListener(index =>
            {
                switch (index)
                {
                    case 0:
                        SelectRoomMap = RoomMap.Map1;
                        break;
                    case 1:
                        SelectRoomMap = RoomMap.Map2;
                        break;
                    case 2:
                        SelectRoomMap = RoomMap.Map3;
                        break;
                    case 3:
                        SelectRoomMap = RoomMap.Random;
                        break;
                    default:
                        break;
                }
            });
        }
        private void InitSearchRoomPanel()
        {
            SR_SearchRoomTipsText = UnityUtil.FindChild<Text>(SearchRoomPanel.transform, nameof(SR_SearchRoomTipsText)) ?? throw new ArgumentNullException(nameof(SR_SearchRoomTipsText));
            SR_ConfirmSearchBtn = UnityUtil.FindChild<Button>(SearchRoomPanel.transform, nameof(SR_ConfirmSearchBtn)) ?? throw new ArgumentNullException(nameof(SR_ConfirmSearchBtn));
            SR_BackLobbyBtn = UnityUtil.FindChild<Button>(SearchRoomPanel.transform, nameof(SR_BackLobbyBtn)) ?? throw new ArgumentNullException(nameof(SR_BackLobbyBtn));
            SR_SearchRoomInput = UnityUtil.FindChild<InputField>(SearchRoomPanel.transform, nameof(SR_SearchRoomInput)) ?? throw new ArgumentNullException(nameof(SR_SearchRoomInput));

            SR_BackLobbyBtn.onClick.AddListener(OnCloseSearchRoomPanel);
        }
        private void InitDialogPanel()
        {
            DL_DialogTitleText = UnityUtil.FindChild<Text>(DialogPanel.transform, nameof(DL_DialogTitleText)) ?? throw new ArgumentNullException(nameof(DL_DialogTitleText));
            DL_DialogTipsText = UnityUtil.FindChild<Text>(DialogPanel.transform, nameof(DL_DialogTipsText)) ?? throw new ArgumentNullException(nameof(DL_DialogTipsText));
            DL_DialogConfirmBtn = UnityUtil.FindChild<Button>(DialogPanel.transform, nameof(DL_DialogConfirmBtn)) ?? throw new ArgumentNullException(nameof(DL_DialogConfirmBtn));
            DL_DialogCancelBtn = UnityUtil.FindChild<Button>(DialogPanel.transform, nameof(DL_DialogCancelBtn)) ?? throw new ArgumentNullException(nameof(DL_DialogCancelBtn));
        }

        public void OnCloseCreateRoomPanel()
        {
            CR_CreateRoomTipsText.text = "";
            CR_MapDropdown.value = 0;
            CR_RoomNameInput.text = "";
            CR_SingleModeToggle.isOn = true;
            CreateRoomPanel.gameObject.SetActive(false);
        }
        public void OnCloseSearchRoomPanel()
        {
            SR_SearchRoomInput.text = "";
            SR_SearchRoomTipsText.text = "";
            SearchRoomPanel.gameObject.SetActive(false);
        }
     
        public void OpenRoomPanel()
        {
            ClearLobbyPanel();
            LobbyPanel.gameObject.SetActive(false);
            RoomPanel.gameObject.SetActive(true);

        }
        private void ClearRoomItems()
        {
            foreach (var item in RL_RoomListScrollView.content.GetComponentsInChildren<Toggle>())
            {
                Destroy(item.gameObject);
            }
        }
        private void AddRoomItems(List<RoomVO> roomList)
        {
            var content = RL_RoomListScrollView.content;
            foreach (var room in roomList)
            {
                GameObject item = Instantiate(_roomItemPrefab, content) as GameObject;
                var toggle = item.GetComponent<Toggle>();
                toggle.group = content.GetComponent<ToggleGroup>();
                var roomItem = item.GetComponent<RoomItem>();
                (roomItem.RoomId, roomItem.RoomName, roomItem.OwnerId, roomItem.OwnerName, roomItem.RoomMode, roomItem.RoomMap, roomItem.PlayerNum) = room;
                toggle.onValueChanged.AddListener(isOn => { if (isOn) UpdateRoomInfo(roomItem); });
                var joinRoomBtn = item.GetComponentInChildren<Button>();
                joinRoomBtn.onClick.AddListener(() => OnJoinRoom?.Invoke(room.RoomID));
            }
            var firstToggle = content.GetComponentInChildren<Toggle>();
            if (firstToggle != null)
            {
                firstToggle.isOn = true;
            }
        }
        public void UpdateRoomItems(List<RoomVO> roomList)
        {
            ClearRoomItems();
            AddRoomItems(roomList);
        }
        private void UpdateRoomInfo(RoomItem roomItem)
        {
            (RI_RoomIDText.text, RI_RoomNameText.text, RI_OwnerNameText.text, RI_RoomModeText.text, RI_RoomMapText.text, RI_PlayerNumText.text) = (roomItem.RoomId, roomItem.RoomName, roomItem.OwnerName, roomItem.RoomMode, roomItem.RoomMap, $"{roomItem.PlayerNum}/8");
        }
        private void ClearLobbyPanel()
        {
            ClearRoomItems();
            (RI_RoomIDText.text, RI_RoomNameText.text, RI_OwnerNameText.text, RI_RoomModeText.text, RI_RoomMapText.text, RI_PlayerNumText.text) = ("", "", "", "", "", "");
        }
    }
}
