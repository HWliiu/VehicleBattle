using GameClient.Common;
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
        public RectTransform TeamModeExtraPanel;
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
        public ScrollRect CP_TeamMessageScrollView { get; set; }
        public Toggle CP_RoomToggle { get; set; }
        public Toggle CP_TeamToggle { get; set; }
        public InputField CP_MessageInput { get; set; }
        public Button CP_SendMessageBtn { get; set; }
        #endregion
        #region TeamModeExtraPanel组件
        public Button TME_ChangeTeamBtn { get; set; }
        #endregion
        #region DialogPanel组件
        public Text DL_DialogTitleText { get; set; }
        public Text DL_DialogTipsText { get; set; }
        public Button DL_DialogConfirmBtn { get; set; }
        public Button DL_DialogCancelBtn { get; set; }
        #endregion

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
        }
        private void InitPlayerListPanel()
        {
            PL_PlayerList1 = UnityUtil.FindChild<GridLayoutGroup>(PlayerListPanel.transform, nameof(PL_PlayerList1)) ?? throw new ArgumentNullException(nameof(PL_PlayerList1));
            PL_PlayerList2 = UnityUtil.FindChild<GridLayoutGroup>(PlayerListPanel.transform, nameof(PL_PlayerList2)) ?? throw new ArgumentNullException(nameof(PL_PlayerList2));
            PL_RoomTipsText = UnityUtil.FindChild<Text>(PlayerListPanel.transform, nameof(PL_RoomTipsText)) ?? throw new ArgumentNullException(nameof(PL_RoomTipsText));
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
            CP_TeamMessageScrollView = UnityUtil.FindChild<ScrollRect>(ChatPanel.transform, nameof(CP_TeamMessageScrollView)) ?? throw new ArgumentNullException(nameof(CP_TeamMessageScrollView));
            CP_RoomToggle = UnityUtil.FindChild<Toggle>(ChatPanel.transform, nameof(CP_RoomToggle)) ?? throw new ArgumentNullException(nameof(CP_RoomToggle));
            CP_TeamToggle = UnityUtil.FindChild<Toggle>(ChatPanel.transform, nameof(CP_TeamToggle)) ?? throw new ArgumentNullException(nameof(CP_TeamToggle));
            CP_MessageInput = UnityUtil.FindChild<InputField>(ChatPanel.transform, nameof(CP_MessageInput)) ?? throw new ArgumentNullException(nameof(CP_MessageInput));
            CP_SendMessageBtn = UnityUtil.FindChild<Button>(ChatPanel.transform, nameof(CP_SendMessageBtn)) ?? throw new ArgumentNullException(nameof(CP_SendMessageBtn));

            CP_RoomToggle.onValueChanged.AddListener(isOn => CP_RoomMessageScrollView.gameObject.SetActive(isOn));
            CP_TeamToggle.onValueChanged.AddListener(isOn => CP_TeamMessageScrollView.gameObject.SetActive(isOn));
        }
        private void InitTeamModeExtraPanel()
        {
            TME_ChangeTeamBtn = UnityUtil.FindChild<Button>(TeamModeExtraPanel.transform, nameof(TME_ChangeTeamBtn)) ?? throw new ArgumentNullException(nameof(TME_ChangeTeamBtn));
        }
        private void InitDialogPanel()
        {
            DL_DialogTitleText = UnityUtil.FindChild<Text>(DialogPanel.transform, nameof(DL_DialogTitleText)) ?? throw new ArgumentNullException(nameof(DL_DialogTitleText));
            DL_DialogTipsText = UnityUtil.FindChild<Text>(DialogPanel.transform, nameof(DL_DialogTipsText)) ?? throw new ArgumentNullException(nameof(DL_DialogTipsText));
            DL_DialogConfirmBtn = UnityUtil.FindChild<Button>(DialogPanel.transform, nameof(DL_DialogConfirmBtn)) ?? throw new ArgumentNullException(nameof(DL_DialogConfirmBtn));
            DL_DialogCancelBtn = UnityUtil.FindChild<Button>(DialogPanel.transform, nameof(DL_DialogCancelBtn)) ?? throw new ArgumentNullException(nameof(DL_DialogCancelBtn));
        }

        // Start is called before the first frame update
        void Start()
        {
            InitRoomPanel();
            InitRoomInfoPanel();
            InitPlayerListPanel();
            InitPlayerInfoPanel();
            InitChatPanel();
            InitTeamModeExtraPanel();
            InitDialogPanel();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnDestroy()
        {

        }

    }
}
