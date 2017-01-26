using UnityEngine;

namespace Script.Camera
{
    public class PivotBasedCameraRig : MonoBehaviour
    {
        // to be placed on the root object of a camera rig
        // 	Camera Rig
        // 		Pivot
        // 			Camera

        protected Transform cameraTransform;
        protected Transform pivotTransform;

        protected virtual void Awake()
        {
            cameraTransform = GetComponentInChildren<UnityEngine.Camera>().transform;
            pivotTransform = cameraTransform.parent;
        }
    }
}
