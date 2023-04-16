using GameClient.Common;
using GameClient.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient.View
{
    public class BattleView : MonoBehaviour
    {
        public RectTransform BattlePanel;
        public RectTransform ESCPanel;
        public RectTransform SettlementPanel;
        public Text ConnStateText;

        public Transform[] SpawnPoints;

        public Button ESC_ContinueBtn { get; set; }
        public Button ESC_BackLobbyBtn { get; set; }
        public Button ESC_ExitGameBtn { get; set; }

        public ScrollRect MP_MessageScrollView { get; set; }

        public Text SP_RankText { get; set; }
        public Text SP_AttainExperienceText { get; set; }
        public Text SP_AttainMoneyText { get; set; }
        public Button SP_BackLobbyBtn { get; set; }

        public Text PI_PlayerNameText { get; set; }
        public Slider PI_HealthyBar { get; set; }

        private GameObject _messageBarPrefab;

        // Start is called before the first frame update
        void Start()
        {
            InitAllPanel();
            AppFacade.Instance.RegisterMediator(new BattleMediator(nameof(BattleMediator), this));

            _messageBarPrefab = Resources.Load<GameObject>("Prefabs/MessageBar");
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ESCPanel.gameObject.SetActive(true);
                PlayerManager.Instance.LocalPlayer.VehicleController.CanControll = false;
            }
        }

        private void OnDestroy() => AppFacade.Instance.RemoveMediator(nameof(BattleMediator));

        private void InitAllPanel()
        {
            ESC_ContinueBtn = UnityUtil.FindChild<Button>(BattlePanel.transform, nameof(ESC_ContinueBtn)) ?? throw new ArgumentNullException(nameof(ESC_ContinueBtn));
            ESC_BackLobbyBtn = UnityUtil.FindChild<Button>(BattlePanel.transform, nameof(ESC_BackLobbyBtn)) ?? throw new ArgumentNullException(nameof(ESC_BackLobbyBtn));
            ESC_ExitGameBtn = UnityUtil.FindChild<Button>(BattlePanel.transform, nameof(ESC_ExitGameBtn)) ?? throw new ArgumentNullException(nameof(ESC_ExitGameBtn));

            MP_MessageScrollView = UnityUtil.FindChild<ScrollRect>(BattlePanel.transform, nameof(MP_MessageScrollView)) ?? throw new ArgumentNullException(nameof(MP_MessageScrollView));

            SP_RankText = UnityUtil.FindChild<Text>(BattlePanel.transform, nameof(SP_RankText)) ?? throw new ArgumentNullException(nameof(SP_RankText));
            SP_AttainExperienceText = UnityUtil.FindChild<Text>(BattlePanel.transform, nameof(SP_AttainExperienceText)) ?? throw new ArgumentNullException(nameof(SP_AttainExperienceText));
            SP_AttainMoneyText = UnityUtil.FindChild<Text>(BattlePanel.transform, nameof(SP_AttainMoneyText)) ?? throw new ArgumentNullException(nameof(SP_AttainMoneyText));
            SP_BackLobbyBtn = UnityUtil.FindChild<Button>(BattlePanel.transform, nameof(SP_BackLobbyBtn)) ?? throw new ArgumentNullException(nameof(SP_BackLobbyBtn));

            PI_PlayerNameText = UnityUtil.FindChild<Text>(BattlePanel.transform, nameof(PI_PlayerNameText)) ?? throw new ArgumentNullException(nameof(PI_PlayerNameText));
            PI_HealthyBar = UnityUtil.FindChild<Slider>(BattlePanel.transform, nameof(PI_HealthyBar)) ?? throw new ArgumentNullException(nameof(PI_HealthyBar));
        }

        public void AddMessageItem(string message)
        {
            GameObject messageBar = Instantiate(_messageBarPrefab, MP_MessageScrollView.content) as GameObject;
            messageBar.transform.Find("PlayerNameText").GetComponent<Text>().text = "系统:";
            messageBar.transform.Find("PlayerMessageText").GetComponent<Text>().text = message;
            StartCoroutine(resetScroll());
            IEnumerator resetScroll() { yield return new WaitForSeconds(0.2f); MP_MessageScrollView.verticalScrollbar.value = 0f; };
        }
    }
}
