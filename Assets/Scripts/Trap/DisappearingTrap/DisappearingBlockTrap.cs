using System.Collections;
using UnityEngine;

namespace CountdownTraps
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class DisappearingBlockTrap : MonoBehaviour
    {
        [Header("Disappearing Block")]
        [SerializeField] private GameObject disappearingBlock;
        [SerializeField, Min(0f)] private float disappearDelay = 0.15f;

        [Header("Trigger")]
        [SerializeField] private bool triggerOnce = true;
        [SerializeField, Min(0f)] private float repeatResetDelay = 2f;

        [Header("Detection Zone")]
        [SerializeField] private BoxCollider detectionZone;
        [SerializeField] private Vector3 detectionCenter;
        [SerializeField] private Vector3 detectionSize = new Vector3(3f, 2f, 3f);

        private bool triggered;
        private bool playerInside;
        private bool blockHidden;
        private Coroutine resetRoutine;

        private void Reset()
        {
            detectionZone = GetComponent<BoxCollider>();
            ApplyDetectionZoneSettings();
        }

        private void Awake()
        {
            ApplyDetectionZoneSettings();

            if (disappearingBlock != null)
            {
                disappearingBlock.SetActive(true);
            }

            blockHidden = false;
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
            blockHidden = false;
            StartCoroutine(DisappearBlock());
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

        private IEnumerator DisappearBlock()
        {
            if (disappearDelay > 0f)
            {
                yield return new WaitForSeconds(disappearDelay);
            }

            if (disappearingBlock != null)
            {
                disappearingBlock.SetActive(false);
            }

            blockHidden = true;
        }

        private IEnumerator ResetAfterPlayerLeaves()
        {
            while (!blockHidden || playerInside)
            {
                yield return null;
            }

            yield return new WaitForSeconds(repeatResetDelay);

            if (!playerInside)
            {
                if (disappearingBlock != null)
                {
                    disappearingBlock.SetActive(true);
                }

                blockHidden = false;
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
