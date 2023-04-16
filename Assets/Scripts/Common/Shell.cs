using GameClient.Controller;
using GameClient.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient.Common
{
    public class Shell : MonoBehaviour
    {
        public ParticleSystem ExplosionParticles;
        public float MaxDamage { get; set; }
        public float ExplosionForce { get; set; } = 1000f;
        public float MaxLifeTime { get; set; } = 2f;
        public float ExplosionRadius { get; set; } = 5f;
        // Start is called before the first frame update
        void Start() => Destroy(gameObject, MaxLifeTime);

        private void OnTriggerEnter(Collider other)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, ExplosionRadius);

            foreach (Collider collider in colliders)
            {
                Rigidbody targetRigidbody = collider.GetComponent<Rigidbody>();

                if (!targetRigidbody)
                    continue;

                targetRigidbody.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius);

                if (!targetRigidbody.CompareTag("Player"))
                    continue;

                int damage = (int)CalculateDamage(targetRigidbody.position);
                PlayerManager.Instance.LocalPlayer.Health -= (int)(damage * (20 - PlayerManager.Instance.LocalPlayer.CurVehicle.Defend) / 10);
            }

            ExplosionParticles.transform.parent = null;
            ExplosionParticles.Play();

            Destroy(ExplosionParticles.gameObject, ExplosionParticles.main.duration);
            Destroy(gameObject);
        }


        private float CalculateDamage(Vector3 targetPosition)
        {
            Vector3 explosionToTarget = targetPosition - transform.position;

            float explosionDistance = explosionToTarget.magnitude;

            float relativeDistance = (ExplosionRadius - explosionDistance) / ExplosionRadius;

            float damage = relativeDistance * MaxDamage;

            damage = Mathf.Max(0f, damage);

            return damage;
        }
    }
}
