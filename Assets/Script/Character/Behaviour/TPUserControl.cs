using Script.Character.Movement;
using Script.Manager;
using UnityEngine;

namespace Script.Character.Behaviour
{
    [RequireComponent(typeof(TPMovement))]
    public class TPUserControl : MonoBehaviour
    {
        private TPMovement character;
        private Transform cam;
        private bool jump;

        private void Awake()
        {
            cam = UnityEngine.Camera.main.transform;
            character = GetComponent<TPMovement>();
        }

        private void Update()
        {
            if (!jump)
                jump = InputManager.Instance.GetButton(InputCode.Jump, InputType.Down);
        }

        private void FixedUpdate()  //called in sync with physics
        {
            float h = InputManager.Instance.GetAxis(InputCode.Horizontal), v = InputManager.Instance.GetAxis(InputCode.Vertical);
            bool crouch = InputManager.Instance.GetButton(InputCode.Crouch, InputType.Instant), walking = InputManager.Instance.GetButton(InputCode.Slow, InputType.Instant);
            var move = (v * Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized) + (h * Vector3.Scale(cam.right, new Vector3(1, 0, 1)).normalized);
            if (walking)
                move *= 0.5f;
            character.Move(move, crouch, jump);
            jump = false;
        }
    }
}
