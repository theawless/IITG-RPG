using UnityEngine;

namespace Script.Camera
{
    public abstract class AbstractTargetFollower : PivotBasedCameraRig
    {
        private enum UpdateType
        {
            FixedUpdate,    // tracking rigidbodies
            LateUpdate,     // tracking objects that are moved in Update
        }
        [SerializeField]
        protected Transform target;
        [SerializeField]
        private bool autoTargetPlayer = true;
        [SerializeField]
        private UpdateType updateType = UpdateType.FixedUpdate;
        [SerializeField]
        private string targetTag = "Player";

        private void AutoTargetPlayer()
        {
            if (autoTargetPlayer && (target == null || !target.gameObject.activeSelf))
            {
                var targetObj = GameObject.FindGameObjectWithTag(targetTag);
                if (targetObj)
                    target = (targetObj.transform);
            }
        }

        private void FixedUpdate()
        {
            AutoTargetPlayer();
            if (updateType == UpdateType.FixedUpdate)
                FollowTarget(Time.deltaTime);
        }

        private void LateUpdate()
        {
            AutoTargetPlayer();
            if (updateType == UpdateType.LateUpdate)
                FollowTarget(Time.deltaTime);
        }

        protected abstract void FollowTarget(float deltaTime);
    }
}
