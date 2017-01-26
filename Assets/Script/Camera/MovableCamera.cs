using Script.Manager;
using System;
using UnityEngine;

namespace Script.Camera
{
    class MovableCamera : PivotBasedCameraRig
    {
        public class CameraPivotTypeUpdateEvent : GameEvent
        {
            public float cameraDistance { get; set; }
            public PivotCameraType cameraType { get; set; }
        }

        public class CameraSideShiftUpdateEvent : GameEvent
        {
            public bool sideShift { get; set; }
        }

        public enum PivotCameraType
        {
            VeryNear,
            Near,
            Far,
            VeryFar,
        }

        [SerializeField]
        private PivotCameraType currentCameraType = PivotCameraType.Far;
        [SerializeField]
        private float changeSpeed = 0.05f;

        [SerializeField]
        private bool _sideShift = false;
        [SerializeField]
        private float sideShiftAmount = 1f;
        private Vector3 initialPivotLocalPosition;
        public bool SideShift
        {
            get
            {
                return _sideShift;
            }
            set
            {
                _sideShift = value;
                EventManager.Instance.TriggerEvent(new CameraSideShiftUpdateEvent { sideShift = SideShift });
            }
        }

        [SerializeField]
        private bool isCrouching = false;
        [SerializeField]
        private float cameraStandHeight = 1.6f;
        [SerializeField]
        private float cameraCrouchHeight = 0.8f;

        private void Update()
        {
            if (InputManager.Instance.GetButton(InputCode.CameraChange, InputType.Up))
            {
                currentCameraType = (PivotCameraType)Mathf.Repeat((int)currentCameraType + 1, Enum.GetValues(typeof(PivotCameraType)).Length);
                EventManager.Instance.TriggerEvent(new CameraPivotTypeUpdateEvent { cameraType = currentCameraType, cameraDistance = GetCameraDistanceByType() });
            }
            isCrouching = InputManager.Instance.GetButton(InputCode.Crouch, InputType.Instant);
        }

        private void LateUpdate()
        {
            HandleSideShift();
            HandleCrouching();
        }

        private void HandleCrouching()
        {
            var y = initialPivotLocalPosition.y + (isCrouching ? cameraCrouchHeight : cameraStandHeight);
            float dummyMoveVelocity = 0;
            var yDamped = Mathf.SmoothDamp(pivotTransform.localPosition.y, y, ref dummyMoveVelocity, changeSpeed);
            pivotTransform.localPosition = new Vector3(pivotTransform.localPosition.x, yDamped, pivotTransform.localPosition.z);
        }

        private void HandleSideShift()
        {
            var x = SideShift ? initialPivotLocalPosition.x + sideShiftAmount : initialPivotLocalPosition.x;
            float dummyMoveVelocity = 0;
            var xDamped = Mathf.SmoothDamp(pivotTransform.localPosition.x, x, ref dummyMoveVelocity, changeSpeed);
            pivotTransform.localPosition = new Vector3(xDamped, pivotTransform.localPosition.y, pivotTransform.localPosition.z);
        }

        private float GetCameraDistanceByType()
        {
            float newCameraDistance;
            switch (currentCameraType)
            {
                case PivotCameraType.VeryNear:
                    newCameraDistance = 1.5f;
                    break;
                case PivotCameraType.Near:
                    newCameraDistance = 2f;
                    break;
                case PivotCameraType.Far:
                default:
                    newCameraDistance = 3f;
                    break;
                case PivotCameraType.VeryFar:
                    newCameraDistance = 3.5f;
                    break;
            }
            return newCameraDistance;
        }
    }
}
