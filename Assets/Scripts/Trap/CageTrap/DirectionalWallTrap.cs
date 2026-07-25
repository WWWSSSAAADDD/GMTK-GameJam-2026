using System.Collections;
using UnityEngine;
using FS_ThirdPerson;

namespace CountdownTraps
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class DirectionalWallTrap : MonoBehaviour
    {
        [Header("Walls")]
        [SerializeField] private GameObject forwardWall;
        [SerializeField] private GameObject rightWall;
        [SerializeField] private GameObject backWall;
        [SerializeField] private GameObject leftWall;

        [Header("Floor")]
        [SerializeField] private GameObject floorPlatform;

        [Header("Reset")]
        [SerializeField, Min(0f)] private float resetDelay = 2f;

        [Header("Input Detection")]
        [Tooltip("World-space direction that activates the forward wall.")]
        [SerializeField] private Vector3 forwardMovementDirection = Vector3.right;
        [Tooltip("World-space direction that activates the right wall.")]
        [SerializeField] private Vector3 rightMovementDirection = Vector3.back;
        [Tooltip("World-space direction that activates the back wall.")]
        [SerializeField] private Vector3 backMovementDirection = Vector3.left;
        [Tooltip("World-space direction that activates the left wall.")]
        [SerializeField] private Vector3 leftMovementDirection = Vector3.forward;
        [SerializeField, Range(0f, 1f)] private float directionDotThreshold = 0.7f;
        [SerializeField, Range(0.01f, 1f)] private float inputThreshold = 0.15f;

        private Transform playerTransform;
        private LocomotionInputManager locomotionInput;
        private bool playerInside;
        private bool forwardWallShown;
        private bool rightWallShown;
        private bool backWallShown;
        private bool leftWallShown;
        private bool floorDropped;
        private Coroutine resetRoutine;

        private void Awake()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            ResetTrap();
        }

        private void Reset()
        {
            GetComponent<BoxCollider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (floorDropped || !TryGetPlayerTransform(other, out Transform player))
            {
                return;
            }

            playerTransform = player;
            locomotionInput = player.GetComponent<LocomotionInputManager>();
            playerInside = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (playerTransform != null && other.GetComponentInParent<CharacterController>()?.transform == playerTransform)
            {
                playerInside = false;
                playerTransform = null;
                locomotionInput = null;

                if (floorDropped)
                {
                    resetRoutine = StartCoroutine(ResetAfterDelay());
                }
            }
        }

        private void Update()
        {
            if (!playerInside || playerTransform == null || floorDropped)
            {
                return;
            }

            if (locomotionInput == null)
            {
                return;
            }

            Vector2 moveInput = locomotionInput.DirectionInput;
            if (moveInput.sqrMagnitude < inputThreshold * inputThreshold)
            {
                return;
            }

            Vector3 inputDirection = playerTransform.right * moveInput.x + playerTransform.forward * moveInput.y;
            inputDirection.y = 0f;
            if (inputDirection.sqrMagnitude > 0f)
            {
                ShowWallForDirection(inputDirection.normalized);
            }
        }

        private void ShowWallForDirection(Vector3 inputDirection)
        {
            float forwardDot = Vector3.Dot(inputDirection, NormalizedOrFallback(forwardMovementDirection, Vector3.right));
            float rightDot = Vector3.Dot(inputDirection, NormalizedOrFallback(rightMovementDirection, Vector3.back));
            float backDot = Vector3.Dot(inputDirection, NormalizedOrFallback(backMovementDirection, Vector3.left));
            float leftDot = Vector3.Dot(inputDirection, NormalizedOrFallback(leftMovementDirection, Vector3.forward));

            float highestDot = Mathf.Max(forwardDot, rightDot, backDot, leftDot);
            if (highestDot < directionDotThreshold)
            {
                return;
            }

            if (highestDot == forwardDot && !forwardWallShown)
            {
                forwardWallShown = true;
                SetActive(forwardWall, true);
            }
            else if (highestDot == rightDot && !rightWallShown)
            {
                rightWallShown = true;
                SetActive(rightWall, true);
            }
            else if (highestDot == backDot && !backWallShown)
            {
                backWallShown = true;
                SetActive(backWall, true);
            }
            else if (highestDot == leftDot && !leftWallShown)
            {
                leftWallShown = true;
                SetActive(leftWall, true);
            }

            if (forwardWallShown && rightWallShown && backWallShown && leftWallShown)
            {
                DropFloor();
            }
        }

        private void DropFloor()
        {
            floorDropped = true;
            SetActive(floorPlatform, false);
        }

        private IEnumerator ResetAfterDelay()
        {
            yield return new WaitForSeconds(resetDelay);
            resetRoutine = null;
            ResetTrap();
        }

        private void ResetTrap()
        {
            if (resetRoutine != null)
            {
                StopCoroutine(resetRoutine);
                resetRoutine = null;
            }

            floorDropped = false;
            forwardWallShown = false;
            rightWallShown = false;
            backWallShown = false;
            leftWallShown = false;
            SetActive(floorPlatform, true);
            SetActive(forwardWall, false);
            SetActive(rightWall, false);
            SetActive(backWall, false);
            SetActive(leftWall, false);
        }

        private static bool TryGetPlayerTransform(Component other, out Transform player)
        {
            CharacterController controller = other.GetComponent<CharacterController>();
            if (controller == null)
            {
                controller = other.GetComponentInParent<CharacterController>();
            }

            player = controller != null ? controller.transform : null;
            return player != null && (other.CompareTag("Player") || player.CompareTag("Player"));
        }

        private static Vector3 NormalizedOrFallback(Vector3 direction, Vector3 fallback)
        {
            return direction.sqrMagnitude > 0f ? direction.normalized : fallback;
        }

        private static void SetActive(GameObject target, bool active)
        {
            if (target != null)
            {
                target.SetActive(active);
            }
        }
    }
}
