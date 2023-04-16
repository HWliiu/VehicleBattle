using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient.Controller
{
    public class CameraController : MonoBehaviour
    {
        private float _distance = 30;
        private readonly float _distanceMin = 10;
        private readonly float _distanceMax = 40;
        private readonly float _slope = 3;
        private readonly float _smooth = 5;
        private readonly float _zoomSpeed = 100;

        private float _distanceAway;
        private float _distanceUp;

        private float _targetDistance;
        private Vector3 _targetPosition;

        public Transform Target;
        private void Start()
        {
            if (Target == null) return;
            _targetDistance = _distance;
            CalculateDistance();
            transform.position = Target.position + Vector3.up * _distanceUp - Target.forward * _distanceAway;
        }
        private void Update()
        {
            _targetDistance -= Input.GetAxis("Mouse ScrollWheel") * _zoomSpeed;
            _targetDistance = Mathf.Clamp(_targetDistance, _distanceMin, _distanceMax);
            _distance = Mathf.Lerp(_distance, _targetDistance, Time.deltaTime * _smooth);
            if (Mathf.Abs(_targetDistance - _distance) > 0.1f) CalculateDistance();
        }
        //private Vector3 _currentVelocity = Vector3.zero;
        private void LateUpdate()
        {
            if (Target == null) return;
            _targetPosition = Target.position + Vector3.up * _distanceUp - Target.forward * _distanceAway;
            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * _smooth);
            //transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentVelocity, 0.1f);
            transform.LookAt(Target);
        }
        void CalculateDistance()
        {
            _distanceAway = Mathf.Sqrt(Mathf.Pow(_distance, 2) / (Mathf.Pow(_slope, 2) + 1));
            _distanceUp = _distanceAway * _slope;
        }
    }
}
