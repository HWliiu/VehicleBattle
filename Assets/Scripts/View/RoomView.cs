using GameClient.Common;
using GameClient.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient.View
{
    public class RoomView : MonoBehaviour
    {
        public RectTransform LobbyPanel;
        public RectTransform RoomPanel;
        public RectTransform RoomInfoPanel;
        public RectTransform PlayerListPanel;
        public RectTransform PlayerInfoPanel;
        public RectTransform ChatPanel;
        //public RectTransform TeamModeExtraPanel;
        public RectTransform DialogPanel;
        #region RoomPanel组件
        public Button RP_PrepareBtn { get; set; }
        public Button RP_ExitRoomBtn { get; set; }
        public Button RP_StartGameBtn { get; set; }
        #endregion
        #region RoomInfoPanel组件
        public Text RI_RoomIdText { get; set; }
        public Text RI_RoomNameText { get; set; }
        public Text RI_RoomModeText { get; set; }
        public Text RI_RoomMapText { get; set; }
        #endregion
        #region PlayerListPanel组件
        public GridLayoutGroup PL_PlayerList1 { get; set; }
        public GridLayoutGroup PL_PlayerList2 { get; set; }
        public Text PL_RoomTipsText { get; set; }
        #endregion
        #region PlayerInfoPanel组件
        public Text PI_PlayerNameText { get; set; }
        public Text PI_VehicleNameText { get; set; }
        public Slider[] PI_VehicleParaSliders { get; set; }
        #endregion
        #region ChatPanel组件
        public ScrollRect CP_RoomMessageScrollView { get; set; }
        //public ScrollRect CP_TeamMessageScrollView { get; set; }
        //public Toggle CP_RoomToggle { get; set; }
        //public Toggle CP_TeamToggle { get; set; }
        public InputField CP_MessageInput { get; set; }
        public Button CP_SendMessageBtn { get; set; }
        #endregion
        #region TeamModeExtraPanel组件
        //public Button TME_ChangeTeamBtn { get; set; }
        #endregion
        #region DialogPanel组件
        public Text DL_DialogTitleText { get; set; }
        public Text DL_DialogTipsText { get; set; }
        public Button DL_DialogConfirmBtn { get; set; }
        public Button DL_DialogCancelBtn { get; set; }
        #endregion

        private GameObject _playerItemPrefab;

        public event Action<string> OnKick;

        private PlayerManager _playerManagerInstance;

        private void InitRoomPanel()
        {
            RP_PrepareBtn = UnityUtil.FindChild<Button>(RoomPanel.transform, nameof(RP_PrepareBtn)) ?? throw new ArgumentNullException(nameof(RP_PrepareBtn));
            RP_ExitRoomBtn = UnityUtil.FindChild<Button>(RoomPanel.transform, nameof(RP_ExitRoomBtn)) ?? throw new ArgumentNullException(nameof(RP_ExitRoomBtn));
            RP_StartGameBtn = UnityUtil.FindChild<Button>(RoomPanel.transform, nameof(RP_StartGameBtn)) ?? throw new ArgumentNullException(nameof(RP_StartGameBtn));
        }
        private void InitRoomInfoPanel()
        {
            RI_RoomIdText = UnityUtil.FindChild<Text>(RoomInfoPanel.transform, nameof(RI_RoomIdText)) ?? throw new ArgumentNullException(nameof(RI_RoomIdText));
            RI_RoomNameText = UnityUtil.FindChild<Text>(RoomInfoPanel.transform, nameof(RI_RoomNameText)) ?? throw new ArgumentNullException(nameof(RI_RoomNameText));
            RI_RoomModeText = UnityUtil.FindChild<Text>(RoomInfoPanel.transform, nameof(RI_RoomModeText)) ?? throw new ArgumentNullException(nameof(RI_RoomModeText));
            RI_RoomMapText = UnityUtil.FindChild<Text>(RoomInfoPanel.transform, nameof(RI_RoomMapText)) ?? throw new ArgumentNullException(nameof(RI_RoomMapText));
        }
        private void InitPlayerListPanel()
        {
            PL_PlayerList1 = UnityUtil.FindChild<GridLayoutGroup>(PlayerListPanel.transform, nameof(PL_PlayerList1)) ?? throw new ArgumentNullException(nameof(PL_PlayerList1));
            PL_PlayerList2 = UnityUtil.FindChild<GridLayoutGroup>(PlayerListPanel.transform, nameof(PL_PlayerList2)) ?? throw new ArgumentNullException(nameof(PL_PlayerList2));
            PL_RoomTipsText = UnityUtil.FindChild<Text>(PlayerListPanel.transform, nameof(PL_RoomTipsText)) ?? throw new ArgumentNullException(nameof(PL_RoomTipsText));

            foreach (var toggle in PL_PlayerList1.GetComponentsInChildren<Toggle>())
            {
                toggle.onValueChanged.AddListener(isOn => { if (isOn) OnSelectPlayerChange(toggle); });
            }
            foreach (var toggle in PL_PlayerList2.GetComponentsInChildren<Toggle>())
            {
                toggle.onValueChanged.AddListener(isOn => { if (isOn) OnSelectPlayerChange(toggle); });
            }

        }
        private void InitPlayerInfoPanel()
        {
            PI_PlayerNameText = UnityUtil.FindChild<Text>(PlayerInfoPanel.transform, nameof(PI_PlayerNameText)) ?? throw new ArgumentNullException(nameof(PI_PlayerNameText));
            PI_VehicleNameText = UnityUtil.FindChild<Text>(PlayerInfoPanel.transform, nameof(PI_VehicleNameText)) ?? throw new ArgumentNullException(nameof(PI_VehicleNameText));
            PI_VehicleParaSliders = PlayerInfoPanel.gameObject.GetComponentsInChildren<Slider>() ?? throw new ArgumentNullException(nameof(PI_VehicleParaSliders));
        }
        private void InitChatPanel()
        {
            CP_RoomMessageScrollView = UnityUtil.FindChild<ScrollRect>(ChatPanel.transform, nameof(CP_RoomMessageScrollView)) ?? throw new ArgumentNullException(nameof(CP_RoomMessageScrollView));
            //CP_TeamMessageScrollView = UnityUtil.FindChild<ScrollRect>(ChatPanel.transform, nameof(CP_TeamMessageScrollView)) ?? throw new ArgumentNullException(nameof(CP_TeamMessageScrollView));
            //CP_RoomToggle = UnityUtil.FindChild<Toggle>(ChatPanel.transform, nameof(CP_RoomToggle)) ?? throw new ArgumentNullException(nameof(CP_RoomToggle));
            //CP_TeamToggle = UnityUtil.FindChild<Toggle>(ChatPanel.transform, nameof(CP_TeamToggle)) ?? throw new ArgumentNullException(nameof(CP_TeamToggle));
            CP_MessageInput = UnityUtil.FindChild<InputField>(ChatPanel.transform, nameof(CP_MessageInput)) ?? throw new ArgumentNullException(nameof(CP_MessageInput));
            CP_SendMessageBtn = UnityUtil.FindChild<Button>(ChatPanel.transform, nameof(CP_SendMessageBtn)) ?? throw new ArgumentNullException(nameof(CP_SendMessageBtn));

            //CP_RoomToggle.onValueChanged.AddListener(isOn => CP_RoomMessageScrollView.gameObject.SetActive(isOn));
            //CP_TeamToggle.onValueChanged.AddListener(isOn => CP_TeamMessageScrollView.gameObject.SetActive(isOn));
        }
        //private void InitTeamModeExtraPanel()
        //{
        //    TME_ChangeTeamBtn = UnityUtil.FindChild<Button>(TeamModeExtraPanel.transform, nameof(TME_ChangeTeamBtn)) ?? throw new ArgumentNullException(nameof(TME_ChangeTeamBtn));
        //}
        private void InitDialogPanel()
        {
            DL_DialogTitleText = UnityUtil.FindChild<Text>(DialogPanel.transform, nameof(DL_DialogTitleText)) ?? throw new ArgumentNullException(nameof(DL_DialogTitleText));
            DL_DialogTipsText = UnityUtil.FindChild<Text>(DialogPanel.transform, nameof(DL_DialogTipsText)) ?? throw new ArgumentNullException(nameof(DL_DialogTipsText));
            DL_DialogConfirmBtn = UnityUtil.FindChild<Button>(DialogPanel.transform, nameof(DL_DialogConfirmBtn)) ?? throw new ArgumentNullException(nameof(DL_DialogConfirmBtn));
            DL_DialogCancelBtn = UnityUtil.FindChild<Button>(DialogPanel.transform, nameof(DL_DialogCancelBtn)) ?? throw new ArgumentNullException(nameof(DL_DialogCancelBtn));

            DL_DialogCancelBtn.onClick.AddListener(OnDialogCancelBtn);
        }

        public void OpenLobbyPanel()
        {
            ClearRoomPanel();
            LobbyPanel.gameObject.SetActive(true);
            RoomPanel.gameObject.SetActive(false);
        }

        // Start is called before the first frame update
        void Start()
        {
            InitRoomPanel();
            InitRoomInfoPanel();
            InitPlayerListPanel();
            InitPlayerInfoPanel();
            InitChatPanel();
            //InitTeamModeExtraPanel();
            InitDialogPanel();

            AppFacade.Instance.RegisterMediator(new RoomMediator(nameof(RoomMediator), this));
            _playerItemPrefab = Resources.Load<GameObject>("Prefab/PlayerItem");
            _playerManagerInstance = PlayerManager.Instance;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnDestroy() => AppFacade.Instance.RemoveMediator(nameof(RoomMediator));

        public void InitRoomInfo(RoomVO room, List<PlayerVO> playerList)
        {
            SetRoomInfo(room);

            switch (room.RoomMode)
            {
                case RoomMode.SingleMode:
                    InitSingleModePlayerList(playerList);
                    break;
                case RoomMode.TeamMode:
                    //InitTeamModePlayerList(room.OwnerId, playerList);
                    break;
                default:
                    break;
            }
        }
        private void InitSingleModePlayerList(List<PlayerVO> playerList)
        {
            foreach (var player in playerList)
            {
                AddPlayerItem(player);
            }
        }
        //private void InitTeamModePlayerList(List<PlayerVO> playerList)
        //{
        //    TeamModeExtraPanel.gameObject.SetActive(true);
        //    var userId = PlayerManager.Instance.LocalPlayer.UserID;
        //    foreach (var player in playerList)
        //    {
        //        Transform grid = null;
        //        switch (player.Team)
        //        {
        //            case Team.None:
        //                throw new Exception();
        //            case Team.Red:
        //                grid = FindEmptyGrid(PL_PlayerList1);
        //                break;
        //            case Team.Blue:
        //                grid = FindEmptyGrid(PL_PlayerList2);
        //                break;
        //            default:
        //                break;
        //        }
        //        if (grid == null) throw new Exception();
        //        GameObject item = Instantiate(_playerItemPrefab, grid) as GameObject;
        //        var playerItem = item.GetComponent<PlayerItem>();
        //        (playerItem.UserName, playerItem.Level, playerItem.PrepareState, playerItem.VehicleName, playerItem.Attack, playerItem.Motility, playerItem.Defend, playerItem.MaxHealth)
        //            = (player.UserName, player.Level.ToString(), player.PrepareState, player.CurVehicle.VehicleName, player.CurVehicle.Attack.ToString(), player.CurVehicle.Motility.ToString(), player.CurVehicle.Defend.ToString(), player.CurVehicle.MaxHealth.ToString());
        //        var kickBtn = item.GetComponentInChildren<Button>(true);
        //        kickBtn.onClick.AddListener(() => OnKick(player.UserID));
        //        if (_isOwner && player.UserID != userId)
        //        {
        //            kickBtn.gameObject.SetActive(true);
        //        }
        //    }
        //}
        public void AddPlayerItem(PlayerVO player)
        {
            var grid = FindEmptyGrid(PL_PlayerList1);
            if (grid == null) grid = FindEmptyGrid(PL_PlayerList2);
            if (grid == null) throw new Exception();
            GameObject item = Instantiate(_playerItemPrefab, grid) as GameObject;
            var playerItem = item.GetComponent<PlayerItem>();

            (playerItem.UserId, playerItem.UserName, playerItem.Level, playerItem.PrepareState, playerItem.VehicleName, playerItem.Attack, playerItem.Motility, playerItem.Defend, playerItem.MaxHealth)
                = (player.UserID, player.UserName, player.Level.ToString(), player.PrepareState, player.CurVehicle.VehicleName, player.CurVehicle.Attack, player.CurVehicle.Motility, player.CurVehicle.Defend, player.CurVehicle.MaxHealth);

            var kickBtn = item.GetComponentInChildren<Button>(true);
            kickBtn.onClick.AddListener(() => OnKick(player.UserID));
            UpdateKickBtnDisplay();
            if (_playerManagerInstance.RoomOwner.UserID == player.UserID)
            {
                item.transform.Find("RoomOwnerText").GetComponent<Text>().gameObject.SetActive(true);
            }
        }
        public void RemovePlayerItem(string playerId)
        {
            foreach (var playerItem in PL_PlayerList1.GetComponentsInChildren<PlayerItem>())
            {
                if (playerItem.UserId == playerId)
                {
                    Destroy(playerItem.gameObject);
                    return;
                }
            }
            foreach (var playerItem in PL_PlayerList2.GetComponentsInChildren<PlayerItem>())
            {
                if (playerItem.UserId == playerId)
                {
                    Destroy(playerItem.gameObject);
                    return;
                }
            }
        }
        private void RemoveAllPlayerItem()
        {
            foreach (var playerItem in PL_PlayerList1.GetComponentsInChildren<PlayerItem>())
            {
                Destroy(playerItem.gameObject);
            }
            foreach (var playerItem in PL_PlayerList2.GetComponentsInChildren<PlayerItem>())
            {
                Destroy(playerItem.gameObject);
            }
        }
        public GameObject FindPlayerItem(string playerId)
        {
            foreach (var playerItem in PL_PlayerList1.GetComponentsInChildren<PlayerItem>())
            {
                if (playerItem.UserId == playerId)
                {
                    return playerItem.gameObject;
                }
            }
            foreach (var playerItem in PL_PlayerList2.GetComponentsInChildren<PlayerItem>())
            {
                if (playerItem.UserId == playerId)
                {
                    return playerItem.gameObject;
                }
            }
            return null;
        }
        private Transform FindEmptyGrid(GridLayoutGroup gridLayout)
        {
            for (int i = 0; i < gridLayout.transform.childCount; i++)
            {
                if (gridLayout.transform.GetChild(i).GetComponentInChildren<PlayerItem>() == null)
                {
                    return gridLayout.transform.GetChild(i).transform;
                }
            }
            return null;
        }
        public void UpdateKickBtnDisplay()
        {
            GameObject button;
            foreach (var playerItem in PL_PlayerList1.GetComponentsInChildren<PlayerItem>())
            {
                button = playerItem.GetComponentInChildren<Button>(true).gameObject;
                button.SetActive(false);
                if (_playerManagerInstance.RoomOwner.UserID == _playerManagerInstance.LocalPlayer.UserID && playerItem.UserId != _playerManagerInstance.LocalPlayer.UserID)
                {
                    button.SetActive(true);
                }
            }
            foreach (var playerItem in PL_PlayerList2.GetComponentsInChildren<PlayerItem>())
            {
                button = playerItem.GetComponentInChildren<Button>(true).gameObject;
                button.SetActive(false);
                if (_playerManagerInstance.RoomOwner.UserID == _playerManagerInstance.LocalPlayer.UserID && playerItem.UserId != _playerManagerInstance.LocalPlayer.UserID)
                {
                    button.SetActive(true);
                }
            }
        }
        private void ClearRoomPanel()
        {
            SetPlayerInfo(null);
            SetRoomInfo(null);
            RemoveAllPlayerItem();
            ClearAllMessageItem();
        }

        private void OnSelectPlayerChange(Toggle toggle)
        {
            var playerItem = toggle.GetComponentInChildren<PlayerItem>();
            SetPlayerInfo(playerItem);
        }

        private void SetPlayerInfo(PlayerItem playerItem)
        {
            if (playerItem != null)
            {
                PI_PlayerNameText.text = playerItem.UserName;
                PI_VehicleNameText.text = playerItem.VehicleName;
                PI_VehicleParaSliders[0].value = playerItem.Attack;
                PI_VehicleParaSliders[0].GetComponentInChildren<Text>().text = playerItem.Attack.ToString();
                PI_VehicleParaSliders[1].value = playerItem.Motility;
                PI_VehicleParaSliders[1].GetComponentInChildren<Text>().text = playerItem.Motility.ToString();
                PI_VehicleParaSliders[2].value = playerItem.Defend;
                PI_VehicleParaSliders[2].GetComponentInChildren<Text>().text = playerItem.Defend.ToString();
                PI_VehicleParaSliders[3].value = playerItem.MaxHealth;
                PI_VehicleParaSliders[3].GetComponentInChildren<Text>().text = playerItem.MaxHealth.ToString();
            }
            else
            {
                PI_PlayerNameText.text = "等待加入...";
                PI_VehicleNameText.text = "- - -";
                PI_VehicleParaSliders[0].value = 0;
                PI_VehicleParaSliders[0].GetComponentInChildren<Text>().text = "0";
                PI_VehicleParaSliders[1].value = 0;
                PI_VehicleParaSliders[1].GetComponentInChildren<Text>().text = "0";
                PI_VehicleParaSliders[2].value = 0;
                PI_VehicleParaSliders[2].GetComponentInChildren<Text>().text = "0";
                PI_VehicleParaSliders[3].value = 0;
                PI_VehicleParaSliders[3].GetComponentInChildren<Text>().text = "0";
            }
        }
        private void SetRoomInfo(RoomVO room)
        {
            if (room != null)
            {
                (RI_RoomIdText.text, RI_RoomNameText.text, RI_RoomModeText.text, RI_RoomMapText.text) = (room.RoomID, room.RoomName, room.RoomMode.ToString(), room.RoomMap.ToString());
            }
            else
            {
                (RI_RoomIdText.text, RI_RoomNameText.text, RI_RoomModeText.text, RI_RoomMapText.text) = ("---", "---", "---", "---");
            }
        }

        private void ClearAllMessageItem()
        {
            var content = CP_RoomMessageScrollView.content;
            for (int i = 0; i < content.transform.childCount; i++)
            {
                var messageBar = content.transform.GetChild(i);
                Destroy(messageBar.gameObject);
            }
        }

        public void OnDialogCancelBtn()
        {
            DL_DialogTitleText.text = "";
            DL_DialogTipsText.text = "";
            DL_DialogConfirmBtn.onClick.RemoveAllListeners();
            DialogPanel.gameObject.SetActive(false);
        }
    }
}
