using System.Collections;
using UnityEngine;

namespace CountdownTraps
{
    [RequireComponent(typeof(Collider))]
    public sealed class MovingPlatformTrap : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private Vector3 moveOffset = new Vector3(0f, 0f, 8f);
        [SerializeField, Min(0.05f)] private float moveDuration = 0.3f;

        [Header("Trigger")]
        [SerializeField] private bool triggerOnce = true;
        [SerializeField, Min(0f)] private float repeatResetDelay = 2f;

        [Header("Audio")]
        [SerializeField] private TrapTriggerAudio triggerAudio = new TrapTriggerAudio();

        [Header("Detection Zone")]
        [SerializeField] private BoxCollider detectionZone;
        [SerializeField] private Vector3 detectionCenter;
        [SerializeField] private Vector3 detectionSize = new Vector3(1f, 2f, 2f);

        private bool triggered;
        private bool playerInside;
        private Vector3 initialPosition;
        private Coroutine moveRoutine;
        private Coroutine resetRoutine;

        public BoxCollider DetectionZone => detectionZone;

        private void Reset()
        {
            BoxCollider[] colliders = GetComponents<BoxCollider>();
            if (colliders.Length > 1)
            {
                detectionZone = colliders[1];
            }

            ApplyDetectionZoneSettings();
        }

        private void Awake()
        {
            initialPosition = transform.position;
            ApplyDetectionZoneSettings();
        }

        private void OnValidate()
        {
            ApplyDetectionZoneSettings();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsPlayer(other))
            {
                return;
            }

            playerInside = true;
            CancelPendingReset();
            if (triggered)
            {
                return;
            }

            triggered = true;
            triggerAudio.Play(GetComponent<AudioSource>());
            moveRoutine = StartCoroutine(MovePlatform());
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsPlayer(other))
            {
                return;
            }

            playerInside = false;
            if (!triggerOnce && triggered && resetRoutine == null)
            {
                resetRoutine = StartCoroutine(ResetAfterDelay());
            }
        }

        private IEnumerator MovePlatform()
        {
            Vector3 startPosition = transform.position;
            Vector3 destination = startPosition + moveOffset;
            float elapsed = 0f;

            while (elapsed < moveDuration)
            {
                elapsed += Time.deltaTime;
                transform.position = Vector3.Lerp(startPosition, destination, elapsed / moveDuration);
                yield return null;
            }

            transform.position = destination;
            moveRoutine = null;
        }

        private IEnumerator ResetAfterDelay()
        {
            yield return new WaitForSeconds(repeatResetDelay);

            if (!playerInside)
            {
                if (moveRoutine != null)
                {
                    StopCoroutine(moveRoutine);
                    moveRoutine = null;
                }

                transform.position = initialPosition;
                triggered = false;
            }

            resetRoutine = null;
        }

        private void CancelPendingReset()
        {
            if (resetRoutine != null)
            {
                StopCoroutine(resetRoutine);
                resetRoutine = null;
            }
        }

        private void ApplyDetectionZoneSettings()
        {
            if (detectionZone == null)
            {
                return;
            }

            detectionZone.isTrigger = true;
            detectionZone.center = detectionCenter;
            detectionZone.size = detectionSize;
        }

        private static bool IsPlayer(Component other)
        {
            return other.CompareTag("Player") || other.GetComponentInParent<CharacterController>() != null;
        }
    }
}
