using GameClient.Controller;
using GameClient.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient.Common
{
    class Bullet : MonoBehaviour
    {
        public float MaxDamage { get; set; }

        public int PanzerInstanceID { get; set; }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player") && PanzerInstanceID != collision.gameObject.GetInstanceID())
            {
                Debug.Log("OnCollisionEnter");
                //PlayerManager.Instance.LocalPlayer.Health -= (int)(MaxDamage * (20 - PlayerManager.Instance.LocalPlayer.CurVehicle.Defend) / 10);
                StartCoroutine(HideBullet());
            }
        }

        private IEnumerator HideBullet()
        {
            yield return new WaitForSeconds(0.2f);
            transform.gameObject.SetActive(false);
        }
    }
}
