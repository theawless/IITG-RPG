using Script.Manager;
using UnityEngine;

namespace Script.Camera
{
    public class FreeLookCam : AbstractTargetFollower
    {
        [SerializeField]
        private float moveSpeed = 1f;
        [Range(0f, 10f)]
        [SerializeField]
        private float turnSpeed = 1.5f;
        [SerializeField]
        private float turnSmoothing = 0.0f;                // to reduce mouse-turn jerkiness
        [SerializeField]
        private float tiltMax = 75f;
        [SerializeField]
        private float tiltMin = 45f;
        [SerializeField]
        private bool invertVertical = false;

        private float lookAngle;                           // The rig's y axis rotation.
        private float tiltAngle;                           // The pivot's x axis rotation.
        private Quaternion pivotTargetRotation;
        private Quaternion transformTargetRotation;

        private void Start()
        {
            pivotTargetRotation = pivotTransform.localRotation;
            transformTargetRotation = transform.localRotation;
        }

        private void Update()
        {
            if (Time.timeScale >= float.Epsilon)
                HandleRotationMovement();
        }

        protected override void FollowTarget(float deltaTime)
        {
            if (target != null)
                transform.position = Vector3.Lerp(transform.position, target.position, deltaTime * moveSpeed);
        }

        private void HandleRotationMovement()
        {
            lookAngle += InputManager.Instance.GetAxis(InputCode.MouseX) * turnSpeed;
            tiltAngle += (invertVertical ? 1 : -1) * InputManager.Instance.GetAxis(InputCode.MouseY) * turnSpeed;
            tiltAngle = Mathf.Clamp(tiltAngle, -tiltMin, tiltMax);

            // HACK: Play with these to have drunk effect
            transformTargetRotation = Quaternion.Euler(0f, InputManager.Instance.GetButton(InputCode.LookBehind,InputType.Instant) ? lookAngle + 180 : lookAngle, 0f);
            pivotTargetRotation = Quaternion.Euler(tiltAngle, 0f, 0f);

            if (turnSmoothing > 0)
            {
                pivotTargetRotation = Quaternion.Slerp(pivotTransform.localRotation, pivotTargetRotation, turnSmoothing * Time.deltaTime);
                transformTargetRotation = Quaternion.Slerp(transform.localRotation, transformTargetRotation, turnSmoothing * Time.deltaTime);
            }
            pivotTransform.localRotation = pivotTargetRotation;
            transform.localRotation = transformTargetRotation;
        }
    }
}
