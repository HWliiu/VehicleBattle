using GameClient.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient.Controller
{
    public sealed class PanzerController : VehicleController
    {
        [SerializeField]
        public GameObject _bulletPrefab;

        private Stack<GameObject> _bulletsPool;
        // Start is called before the first frame update
        void Start()
        {
            Init();

            _bulletsPool = new Stack<GameObject>();
            for (int i = 0; i < 30; i++)
            {
                var bullet = Instantiate(_bulletPrefab);
                bullet.SetActive(false);
                _bulletsPool.Push(bullet);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (_isLocalPlayer)
            {
                if (CanControll)
                {
                    LocalPlayerMove();
                    LocalPlayerAim();
                }
            }
            else
            {
                NetPlayerMove();
                NetPlayerAim();
            }
        }
        private void FixedUpdate()
        {
            if (_isLocalPlayer)
            {
                _moveInputValue = Input.GetAxis("Vertical");
                _turnInputValue = Input.GetAxis("Horizontal");
            }
        }

        protected override void Init() => base.Init();

        protected override void LocalPlayerMove()
        {
            base.LocalPlayerMove();
        }
        protected override void LocalPlayerAim()
        {
            base.LocalPlayerAim();

            if (Input.GetButton("Fire1"))
            {
                if (_cooling) return;
                LocalPlayerFire();
            }
        }

        protected override void LocalPlayerFire()
        {
            base.LocalPlayerFire();

            _cooling = true;
            IEnumerator enumerator()
            {
                yield return new WaitForSeconds(0.2f);
                _cooling = false;
            }
            StartCoroutine(enumerator());

            var bulletInstance = _bulletsPool.Pop();
            if (bulletInstance == null) return;
            bulletInstance.transform.position = _fireTransform.position + Random.insideUnitSphere * 0.2f;
            bulletInstance.transform.rotation = _fireTransform.rotation;
            bulletInstance.SetActive(true);
            bulletInstance.GetComponent<Rigidbody>().velocity = 100 * _fireTransform.forward;
            bulletInstance.GetComponent<Bullet>().MaxDamage = Attack;
            bulletInstance.GetComponent<Bullet>().PanzerInstanceID = GetInstanceID();
            IEnumerator recyleBullet()
            {
                yield return new WaitForSeconds(3f);
                bulletInstance.SetActive(false);
                _bulletsPool.Push(bulletInstance);
            }
            StartCoroutine(recyleBullet());

            OnFire?.Invoke(_fireTransform.position, _fireTransform.rotation, 100 * _fireTransform.forward);
        }


        protected override void NetPlayerAim()
        {
            base.NetPlayerAim();
        }

        protected override void NetPlayerMove()
        {
            base.NetPlayerMove();
        }
        public override void NetPlayerFire(Vector3 position, Quaternion rotation, Vector3 velocity)
        {
            base.NetPlayerFire(position, rotation, velocity);
            var bulletInstance = _bulletsPool.Pop();
            if (bulletInstance == null) return;
            bulletInstance.transform.position = position;
            bulletInstance.transform.rotation = rotation;
            bulletInstance.SetActive(true);
            bulletInstance.GetComponent<Rigidbody>().velocity = velocity;
            bulletInstance.GetComponent<Bullet>().MaxDamage = Attack;
            bulletInstance.GetComponent<Bullet>().PanzerInstanceID = GetInstanceID();
            IEnumerator recyleBullet()
            {
                yield return new WaitForSeconds(3f);
                bulletInstance.SetActive(false);
                _bulletsPool.Push(bulletInstance);
            }
            StartCoroutine(recyleBullet());
        }

        private void OnDestroy()
        {
            Destroy(_aimLocator);
            foreach (var bullet in _bulletsPool)
            {
                Destroy(bullet);
            }
        }
    }
}
