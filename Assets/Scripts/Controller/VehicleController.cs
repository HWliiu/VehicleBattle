using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient.Controller
{
    public abstract class VehicleController : MonoBehaviour
    {
        public bool IsLocalPlayer
        {
            get => _isLocalPlayer;
            set
            {
                //_rigidbody.isKinematic = !value;
                _isLocalPlayer = value;
                IEnumerator enumerator()
                {
                    yield return null;
                    _rigidbody.isKinematic = !value;
                }
                StartCoroutine(enumerator());
                if (_isLocalPlayer)
                {
                    StartCoroutine(UploadTransform());
                }
            }
        }
        public float MoveInputValue
        {
            get => _moveInputValue;
            set
            {
                if (_isLocalPlayer) return;
                _moveInputValue = value;
            }
        }
        public float TurnInputValue
        {
            get => _turnInputValue;
            set
            {
                if (_isLocalPlayer) return;
                _turnInputValue = value;
            }
        }
        public Vector3 VehiclePosition { get; set; }
        public Quaternion VehicleRotation { get; set; }
        public Quaternion TurretRotation { get; set; }

        public bool CanControll { get; set; } = true;

        protected float _moveInputValue;
        protected float _turnInputValue;

        public float MoveSpeed { protected get; set; } = 12f;
        public float TurnSpeed { protected get; set; } = 180f;
        public float AimSpeed { protected get; set; } = 2f;
        public float Attack { protected get; set; } = 10f;

        protected Rigidbody _rigidbody;

        [SerializeField]
        protected Transform _turretTransform;
        [SerializeField]
        protected Transform _fireTransform;
        [SerializeField]
        protected GameObject _vehicleExplosion;
        [SerializeField]
        protected LineRenderer _aimLine;
        [SerializeField]
        protected Texture _aimTexture;

        protected bool _cooling;
        protected Vector3 _lockAimPoint;
        protected GameObject _aimLocator;

        protected Gradient _normalGradient;
        protected Gradient _aimedGradient;

        public Action<Vector3, Quaternion, Quaternion, float, float> OnMove;
        public Action<Vector3, Quaternion, Vector3> OnFire;

        protected bool _isLocalPlayer;
        protected WaitForSeconds _uploadTransformInterval;
        protected bool _vehicleTransformChangedFlag;
        protected bool _turretTransformChangedFlag;

        protected virtual void Init()
        {
            _rigidbody = GetComponent<Rigidbody>();

            _normalGradient = new Gradient();
            _normalGradient.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.yellow, 0), new GradientColorKey(Color.white, 1) }, new GradientAlphaKey[] { new GradientAlphaKey(0.1f, 0), new GradientAlphaKey(0.3f, 1) });
            _aimedGradient = new Gradient();
            _aimedGradient.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.green, 0), new GradientColorKey(Color.blue, 1) }, new GradientAlphaKey[] { new GradientAlphaKey(0.5f, 0), new GradientAlphaKey(0.5f, 1) });

            _aimLocator = Instantiate(Resources.Load<GameObject>("Prefabs/Locator"));
            _aimLocator.SetActive(false);

            VehiclePosition = transform.position;
            VehicleRotation = transform.rotation;
            TurretRotation = _turretTransform.rotation;

            _uploadTransformInterval = new WaitForSeconds(0.02f);
        }

        protected virtual void LocalPlayerMove()
        {
            _rigidbody.MovePosition(_rigidbody.position + (transform.forward * _moveInputValue * MoveSpeed * Time.deltaTime));
            _rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.Euler(0f, _turnInputValue * TurnSpeed * Time.deltaTime, 0f));

            _vehicleTransformChangedFlag = VehiclePosition != transform.position || VehicleRotation != transform.rotation;
            if (_vehicleTransformChangedFlag)
            {
                VehicleRotation = transform.rotation;
                VehiclePosition = transform.position;
            }
        }

        protected virtual void LocalPlayerAim()
        {
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Debug.DrawRay(camRay.origin, camRay.direction * 100, Color.blue);
            if (Physics.Raycast(camRay, out var floorHit))
            {
                //Debug.DrawRay(floorHit.point, Vector3.up * _turretTransform.position.y, Color.green);
                if (Input.GetButtonDown("Fire2")) _lockAimPoint = floorHit.point;
                Vector3 turretToMouse = Input.GetButton("Fire2") ? _lockAimPoint - _turretTransform.position : floorHit.point - _turretTransform.position;
                turretToMouse.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(turretToMouse);
                _turretTransform.rotation = Quaternion.Lerp(_turretTransform.rotation, targetRotation, Time.deltaTime * AimSpeed);

                if (Input.GetButtonDown("Fire2"))
                {
                    _aimLine.enabled = true;
                    _aimLocator.transform.position = _lockAimPoint;
                    _aimLocator.SetActive(true);
                }
                else if (Input.GetButton("Fire2"))
                {
                    Ray aimRay = new Ray(_turretTransform.position, _turretTransform.forward);
                    if (Physics.Raycast(aimRay, out var aimRayHit))
                    {
                        _aimLine.colorGradient = aimRayHit.transform.CompareTag("Player") ? _aimedGradient : _normalGradient;
                        _aimLine.SetPositions(new Vector3[] { aimRay.origin, aimRayHit.point });
                    }
                    else
                    {
                        _aimLine.colorGradient = _normalGradient;
                        _aimLine.SetPositions(new Vector3[] { aimRay.origin, aimRay.GetPoint(100) });
                    }
                }
                else if (Input.GetButtonUp("Fire2"))
                {
                    _aimLine.enabled = false;
                    _aimLocator.SetActive(false);
                }
            }

            _turretTransformChangedFlag = TurretRotation != _turretTransform.rotation;
            if (_turretTransformChangedFlag) TurretRotation = _turretTransform.rotation;
        }

        protected virtual void LocalPlayerFire()
        {
        }
        protected virtual IEnumerator UploadTransform()
        {
            while (true)
            {
                yield return _uploadTransformInterval;
                if (_vehicleTransformChangedFlag || _turretTransformChangedFlag)
                {
                    OnMove?.Invoke(VehiclePosition, VehicleRotation, TurretRotation, MoveInputValue, TurnInputValue);
                }
            }
        }

        protected virtual void NetPlayerMove()
        {
            transform.position = Vector3.Lerp(transform.position, VehiclePosition, Time.deltaTime * 10);
            transform.rotation = Quaternion.Lerp(transform.rotation, VehicleRotation, Time.deltaTime * 10);
        }

        protected virtual void NetPlayerAim() => _turretTransform.rotation = Quaternion.Lerp(_turretTransform.rotation, TurretRotation, Time.deltaTime * 10);

        public virtual void NetPlayerFire(Vector3 position, Quaternion rotation, Vector3 velocity)
        {
        }

        //public void OnGUI()
        //{
        //    if (IsLocalPlayer && Input.GetButton("Fire2"))
        //    {
        //        var position = Camera.main.WorldToScreenPoint(new Vector3(_lockAimPoint.x, _turretTransform.position.y, _lockAimPoint.z));
        //        Rect rect2 = new Rect(position.x - (_aimTexture.width >> 1), Screen.height - position.y - (_aimTexture.height >> 1), _aimTexture.width, _aimTexture.height);
        //        GUI.DrawTexture(rect2, _aimTexture);
        //    }
        //}
        public virtual void Explosion()
        {
            Destroy(transform.gameObject);
            var explosion = Instantiate(_vehicleExplosion, transform.position, transform.rotation);
            explosion.GetComponent<ParticleSystem>().Play();
            Destroy(explosion, 3);
        }

    }
}
