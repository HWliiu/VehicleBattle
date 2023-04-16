using GameClient.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient.Controller
{
    public sealed class TankController : VehicleController
    {
        [SerializeField]
        public Slider _aimSlider;

        [SerializeField]
        public GameObject _shellPrefab;

        private float _minLaunchForce = 15f;
        private float _maxLaunchForce = 30f;
        private float _maxChargeTime = 0.75f;

        private float _currentLaunchForce;

        private float _chargeSpeed;
        // Start is called before the first frame update
        void Start()
        {
            Init();
            _currentLaunchForce = _minLaunchForce;
            _aimSlider.value = _minLaunchForce;
            _chargeSpeed = (_maxLaunchForce - _minLaunchForce) / _maxChargeTime;
            _aimSlider.GetComponentInChildren<Image>().enabled = false;
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

        protected override void LocalPlayerMove() => base.LocalPlayerMove();

        protected override void LocalPlayerAim()
        {
            base.LocalPlayerAim();
            _aimSlider.transform.parent.rotation = Quaternion.Euler(_turretTransform.rotation.eulerAngles + new Vector3(90, 0, 0));
            Charge();
        }

        protected override void LocalPlayerFire()
        {
            _cooling = true;
            IEnumerator enumerator()
            {
                yield return new WaitForSeconds(2);
                _cooling = false;
            }
            StartCoroutine(enumerator());

            var shellInstance = Instantiate(_shellPrefab, _fireTransform.position, _fireTransform.rotation) as GameObject;
            Vector3 force = _currentLaunchForce * _fireTransform.forward;
            shellInstance.GetComponent<Rigidbody>().velocity = force;
            shellInstance.GetComponent<Shell>().MaxDamage = Attack * 20;
            _rigidbody.AddForceAtPosition(-force, _fireTransform.position, ForceMode.Impulse);
            _currentLaunchForce = _minLaunchForce;

            OnFire?.Invoke(_fireTransform.position, _fireTransform.rotation, force);
        }

        protected override void NetPlayerMove()
        {
            base.NetPlayerMove();
        }

        protected override void NetPlayerAim()
        {
            base.NetPlayerAim();
        }

        public override void NetPlayerFire(Vector3 position, Quaternion rotation, Vector3 velocity)
        {
            base.NetPlayerFire(position, rotation, velocity);
            GameObject shellInstance = Instantiate(_shellPrefab, position, rotation) as GameObject;
            shellInstance.GetComponent<Rigidbody>().velocity = velocity;
            shellInstance.GetComponent<Shell>().MaxDamage = Attack * 20;
        }

        private void Charge()
        {
            if (_cooling)
            {
                return;
            }

            _aimSlider.value = _minLaunchForce;
            if (_currentLaunchForce >= _maxLaunchForce)
            {
                _currentLaunchForce = _maxLaunchForce;
                LocalPlayerFire();
            }
            else if (Input.GetButtonDown("Fire1"))
            {
                _currentLaunchForce = _minLaunchForce;
                _aimSlider.GetComponentInChildren<Image>().enabled = true;
            }
            else if (Input.GetButton("Fire1"))
            {
                _currentLaunchForce += _chargeSpeed * Time.deltaTime;
                _aimSlider.value = _currentLaunchForce;
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                LocalPlayerFire();
                _aimSlider.GetComponentInChildren<Image>().enabled = false;
            }
        }

        private void OnDestroy()
        {
            Destroy(_aimLocator);
        }
    }
}
