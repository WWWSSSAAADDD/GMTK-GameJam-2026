using System.Collections;
using UnityEngine;

namespace CountdownTraps
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class AppearingObstacleTrap : MonoBehaviour
    {
        [Header("Appearing Obstacle")]
        [SerializeField] private GameObject appearingObstacle;
        [SerializeField, Min(0f)] private float appearDelay = 0f;

        [Header("Trigger")]
        [SerializeField] private bool triggerOnce = true;
        [SerializeField, Min(0f)] private float repeatResetDelay = 2f;

        [Header("Detection Zone")]
        [SerializeField] private BoxCollider detectionZone;
        [SerializeField] private Vector3 detectionCenter;
        [SerializeField] private Vector3 detectionSize = new Vector3(3f, 2f, 2f);

        private bool triggered;
        private bool playerInside;
        private bool obstacleShown;
        private Coroutine resetRoutine;

        private void Reset()
        {
            detectionZone = GetComponent<BoxCollider>();
            ApplyDetectionZoneSettings();
        }

        private void Awake()
        {
            ApplyDetectionZoneSettings();

            if (Application.isPlaying && appearingObstacle != null)
            {
                appearingObstacle.SetActive(false);
            }

            obstacleShown = false;
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
            StartCoroutine(AppearObstacle());
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
                resetRoutine = StartCoroutine(ResetAfterPlayerLeaves());
            }
        }

        private IEnumerator AppearObstacle()
        {
            if (appearDelay > 0f)
            {
                yield return new WaitForSeconds(appearDelay);
            }

            if (appearingObstacle != null)
            {
                appearingObstacle.SetActive(true);
            }

            obstacleShown = true;
        }

        private IEnumerator ResetAfterPlayerLeaves()
        {
            while (!obstacleShown || playerInside)
            {
                yield return null;
            }

            yield return new WaitForSeconds(repeatResetDelay);

            if (!playerInside)
            {
                if (appearingObstacle != null)
                {
                    appearingObstacle.SetActive(false);
                }

                obstacleShown = false;
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
