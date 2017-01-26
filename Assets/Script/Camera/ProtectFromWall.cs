using Script.Manager;
using System;
using UnityEngine;

namespace Script.Camera
{
    public class ProtectFromWall : PivotBasedCameraRig
    {
        public bool protectingFromSomething { get; private set; }    // used for determining if there is an object between the target and the camera

        [SerializeField]
        private float clipMoveTime = 0.05f;              // time taken to move when avoiding
        [SerializeField]
        private float usualMoveTime = 0.1f;                 // time taken to move towards desired position
        [SerializeField]
        private float sphereCastRadius = 0.1f;
        [SerializeField]
        private float closestDistance = 0.5f;
        [SerializeField]
        private string dontClipTag = "Player";

        [SerializeField]
        private float originalDistance;             // the original distance to the camera before any modification are made
        private float currentDistance;              // the current distance from the camera to the target

        private void Start()
        {
            originalDistance = currentDistance = cameraTransform.localPosition.magnitude;
            EventManager.Instance.AddListener<MovableCamera.CameraPivotTypeUpdateEvent>(CameraPivotTypeUpdated);
        }

        private void CameraPivotTypeUpdated(MovableCamera.CameraPivotTypeUpdateEvent e)
        {
            originalDistance = e.cameraDistance;
        }

        private void LateUpdate()
        {
            var ray = new Ray
            {
                origin = pivotTransform.position + pivotTransform.forward * sphereCastRadius,
                direction = -pivotTransform.forward
            };
            RaycastHit[] rayHits;

            // initial check to see if start of spherecast intersects anything
            if (InitialCheck(ray.origin))
            {
                ray.origin += pivotTransform.forward * sphereCastRadius;
                rayHits = Physics.RaycastAll(ray, originalDistance - sphereCastRadius);
            }
            else
            {
                // if there was no collision do a sphere cast to see if there were any other collisions
                rayHits = Physics.SphereCastAll(ray, sphereCastRadius, originalDistance + sphereCastRadius);
            }
            var targetDistance = FindTargetDistance(rayHits);
            if (protectingFromSomething)
                Debug.DrawRay(ray.origin, -pivotTransform.forward * (targetDistance + sphereCastRadius), Color.red);

            // hit something so move the camera to a better position
            float dummyMoveVelocity = 0;
            currentDistance = Mathf.SmoothDamp(currentDistance, targetDistance, ref dummyMoveVelocity, (currentDistance > targetDistance) && protectingFromSomething ? clipMoveTime : usualMoveTime);
            currentDistance = Math.Max(currentDistance, closestDistance);
            cameraTransform.localPosition = cameraTransform.localPosition.normalized * currentDistance;
        }

        private float FindTargetDistance(RaycastHit[] rayHits)
        {
            protectingFromSomething = false;
            Array.Sort(rayHits, (x, y) => x.distance.CompareTo(y.distance));
            float nearestRay = Mathf.Infinity, targetDistance = originalDistance;
            foreach (var rayHit in rayHits)
            {
                if (rayHit.distance < nearestRay && IsCollisionValid(rayHit.collider))
                {
                    nearestRay = rayHit.distance;
                    targetDistance = -pivotTransform.InverseTransformPoint(rayHit.point).z;
                    protectingFromSomething = true;
                }
            }
            return targetDistance;
        }

        private bool InitialCheck(Vector3 origin)
        {
            foreach (var collider in Physics.OverlapSphere(origin, sphereCastRadius))
                if (IsCollisionValid(collider))
                    return true;
            return false;
        }

        private bool IsCollisionValid(Collider collider)
        {
            return (!collider.isTrigger) && !(collider.attachedRigidbody != null && collider.attachedRigidbody.CompareTag(dontClipTag));
        }
    }
}