using UnityEngine;

namespace Script.Character.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class TPMovement : MonoBehaviour
    {
        [SerializeField]
        private float movingTurnSpeed = 360;
        [SerializeField]
        private float stationaryTurnSpeed = 180;
        [SerializeField]
        private float jumpPower = 12f;
        [Range(1f, 4f)]
        [SerializeField]
        private float gravityMultiplier = 2f;
        [SerializeField]
        private float runCycleLegOffset = 0.2f;         //specific to a character
        [SerializeField]
        private float moveSpeedMultiplier = 1f;
        [SerializeField]
        private float animSpeedMultiplier = 1f;
        [SerializeField]
        private float groundCheckDistance = 0.1f;

        private const float Half = 0.5f;

        [SerializeField]
        private bool isGrounded;
        private Rigidbody rigidBody;
        private Animator animator;
        private float originalGroundCheckDistance;
        private float turnAmount;
        private float forwardAmount;
        private Vector3 groundNormal;
        private float capsuleDefaultHeight;
        private Vector3 capsuleDefaultCenter;
        private CapsuleCollider capsule;
        private bool crouching;

        private void Awake()
        {
            SetupAnimator();
            rigidBody = GetComponent<Rigidbody>();
            capsule = GetComponent<CapsuleCollider>();
            rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            capsuleDefaultHeight = capsule.height;
            capsuleDefaultCenter = capsule.center;
            originalGroundCheckDistance = groundCheckDistance;
        }

        private void SetupAnimator()
        {
            animator = GetComponent<Animator>();
            foreach (var childAnimator in GetComponentsInChildren<Animator>())
            {
                if (animator != childAnimator)
                {
                    animator.avatar = childAnimator.avatar;
                    Destroy(childAnimator);
                }
            }
        }

        public void Move(Vector3 move, bool crouch, bool jump)
        {
            CheckGroundStatus();
            // convert the world relative moveInput vector into a local-relative
            if (move.magnitude > 1f)
                move.Normalize();
            move = transform.InverseTransformDirection(move);
            move = Vector3.ProjectOnPlane(move, groundNormal);
            // turn amount and forward amount required to head in the desired direction.
            turnAmount = Mathf.Atan2(move.x, move.z);
            forwardAmount = move.z;
            ApplyExtraTurnRotation();

            if (isGrounded)
                HandleGroundedMovement(crouch, jump);
            else
                HandleAirborneMovement();

            ScaleCapsuleForCrouching(crouch);
            UpdateAnimator(move);
        }

        private void ScaleCapsuleForCrouching(bool crouch)
        {
            if (isGrounded && crouch)
            {
                if (!crouching)
                {
                    capsule.height *= Half;
                    capsule.center *= Half;
                    crouching = true;
                }
            }
            else
            {
                crouching = IsStandingInLowHeadroom();
                if (!crouching)
                {
                    capsule.height = capsuleDefaultHeight;
                    capsule.center = capsuleDefaultCenter;
                }
            }
        }

        private bool IsStandingInLowHeadroom()
        {
            var crouchRay = new Ray(rigidBody.position + Vector3.up * capsule.radius * Half, Vector3.up);
            var crouchRayLength = capsuleDefaultHeight - capsule.radius * Half;
            return (Physics.SphereCast(crouchRay, capsule.radius * Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore));
        }

        private void UpdateAnimator(Vector3 move)
        {
            animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
            animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
            animator.SetBool("Crouch", crouching);
            animator.SetBool("OnGround", isGrounded);
            if (!isGrounded)
                animator.SetFloat("Jump", rigidBody.velocity.y);

            // calculate which leg is behind, so as to leave that leg trailing in the jump animation
            // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
            var runCycle = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime + runCycleLegOffset, 1);
            var jumpLeg = (runCycle < Half ? 1 : -1) * forwardAmount;
            if (isGrounded)
                animator.SetFloat("JumpLeg", jumpLeg);

            // magnifies the movement speed because of the root motion, don't use that while airborne
            animator.speed = (isGrounded && move.magnitude > 0) ? animSpeedMultiplier : 1;
        }

        private void HandleAirborneMovement()
        {
            // apply extra gravity from multiplier
            rigidBody.AddForce((Physics.gravity * gravityMultiplier) - Physics.gravity);
            groundCheckDistance = rigidBody.velocity.y < 0 ? originalGroundCheckDistance : 0.01f;
        }

        private void HandleGroundedMovement(bool crouch, bool jump)
        {
            if (jump && !crouch && animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
            {
                rigidBody.velocity = new Vector3(rigidBody.velocity.x, jumpPower, rigidBody.velocity.z);
                isGrounded = animator.applyRootMotion = false;
                groundCheckDistance = 0.1f;
            }
        }

        private void ApplyExtraTurnRotation()
        {
            // this is in addition to root rotation of animation
            var turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
            transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
        }

        // override the default root motion to modify the positional speed before it's applied
        private void OnAnimatorMove()
        {
            if (isGrounded && Time.deltaTime > 0)
            {
                var velocity = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;
                // preserve the existing y
                velocity.y = rigidBody.velocity.y;
                rigidBody.velocity = velocity;
            }
        }

        private void CheckGroundStatus()
        {
            RaycastHit hitInfo;
            // 0.1f is a small offset to start the ray from inside the character
            var origin = transform.position + (Vector3.up * 0.1f);
            var hitSomething = Physics.Raycast(origin, Vector3.down, out hitInfo, groundCheckDistance);
            isGrounded = animator.applyRootMotion = hitSomething;
            groundNormal = hitSomething ? hitInfo.normal : Vector3.up;
            Debug.DrawLine(origin, origin + (Vector3.down * groundCheckDistance), Color.red);
        }
    }
}