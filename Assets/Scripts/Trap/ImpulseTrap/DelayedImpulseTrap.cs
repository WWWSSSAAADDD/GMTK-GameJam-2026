using System.Collections;
using UnityEngine;

namespace CountdownTraps
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class DelayedImpulseTrap : MonoBehaviour
    {
        [Header("Delay")]
        [SerializeField, Min(0f)] private float activationDelay = 0.8f;

        [Header("Effect Zone")]
        [SerializeField] private Vector3 effectCenter = new Vector3(0f, 1.2f, 2f);
        [SerializeField] private Vector3 effectSize = new Vector3(4f, 3f, 4f);

        [Header("Impulse")]
        [Tooltip("Local direction. Rotate this Trap to rotate the impulse direction with it.")]
        [SerializeField] private Vector3 impulseDirection = new Vector3(0f, 0.65f, 1f);
        [SerializeField, Min(0f)] private float impulseSpeed = 14f;
        [SerializeField, Min(0.02f)] private float impulseDuration = 0.35f;
        
        [Header("Debug")]
        [SerializeField, Min(0.1f)] private float impulseDebugArrowLength = 4f;

        [Header("Activation Feedback")]
        [Tooltip("Optional visual shown or hidden by this trap. Extend PlayActivationFeedback to play an animation or sound.")]
        [SerializeField] private GameObject activationVisual;
        [SerializeField] private bool hideActivationVisualOnStart = true;
        [Tooltip("Optional position marker for the activation animation. It is drawn as a magenta gizmo.")]
        [SerializeField] private Transform activationAnimationPoint;

        [Header("Trigger")]
        [SerializeField] private bool triggerOnce = true;
        [SerializeField, Min(0f)] private float repeatResetDelay = 2f;

        [Header("Detection Zone")]
        [SerializeField] private BoxCollider detectionZone;
        [SerializeField] private Vector3 detectionCenter;
        [SerializeField] private Vector3 detectionSize = new Vector3(3f, 2f, 2f);

        private bool triggered;
        private bool playerInsideDetection;
        private bool activationComplete;
        private CharacterController trackedPlayer;
        private Coroutine activationRoutine;
        private Coroutine resetRoutine;

        public BoxCollider DetectionZone => detectionZone;

        private void Reset()
        {
            detectionZone = GetComponent<BoxCollider>();
            ApplyDetectionZoneSettings();
        }

        private void Awake()
        {
            ApplyDetectionZoneSettings();

            if (Application.isPlaying && hideActivationVisualOnStart && activationVisual != null)
            {
                activationVisual.SetActive(false);
            }
        }

        private void OnValidate()
        {
            effectSize = new Vector3(
                Mathf.Max(0.01f, effectSize.x),
                Mathf.Max(0.01f, effectSize.y),
                Mathf.Max(0.01f, effectSize.z));
            ApplyDetectionZoneSettings();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!TryGetPlayerController(other, out CharacterController playerController))
            {
                return;
            }

            playerInsideDetection = true;
            trackedPlayer = playerController;
            CancelPendingReset();

            if (triggered)
            {
                return;
            }

            triggered = true;
            activationComplete = false;
            PlayActivationFeedback();
            activationRoutine = StartCoroutine(ActivateAfterDelay());
        }

        private void OnTriggerExit(Collider other)
        {
            if (!TryGetPlayerController(other, out CharacterController playerController) || playerController != trackedPlayer)
            {
                return;
            }

            playerInsideDetection = false;
            TryScheduleReset();
        }

        private IEnumerator ActivateAfterDelay()
        {
            if (activationDelay > 0f)
            {
                yield return new WaitForSeconds(activationDelay);
            }

            if (IsPlayerInsideEffectZone(trackedPlayer))
            {
                yield return ApplyImpulse(trackedPlayer);
            }

            activationRoutine = null;
            activationComplete = true;
            TryScheduleReset();
        }

        private IEnumerator ApplyImpulse(CharacterController playerController)
        {
            Vector3 worldDirection = transform.TransformDirection(impulseDirection);
            if (worldDirection.sqrMagnitude < 0.0001f || impulseSpeed <= 0f)
            {
                yield break;
            }

            worldDirection.Normalize();
            float elapsed = 0f;

            while (playerController != null && elapsed < impulseDuration)
            {
                elapsed += Time.deltaTime;
                float remainingStrength = 1f - Mathf.Clamp01(elapsed / impulseDuration);
                playerController.Move(worldDirection * impulseSpeed * remainingStrength * Time.deltaTime);
                yield return null;
            }
        }

        private bool IsPlayerInsideEffectZone(CharacterController playerController)
        {
            if (playerController == null)
            {
                return false;
            }

            Vector3 halfExtents = Vector3.Scale(effectSize, Abs(transform.lossyScale)) * 0.5f;
            Collider[] colliders = Physics.OverlapBox(
                transform.TransformPoint(effectCenter),
                halfExtents,
                transform.rotation,
                ~0,
                QueryTriggerInteraction.Collide);

            foreach (Collider collider in colliders)
            {
                if (collider == playerController || collider.GetComponentInParent<CharacterController>() == playerController)
                {
                    return true;
                }
            }

            return false;
        }

        private void TryScheduleReset()
        {
            if (triggerOnce || !triggered || !activationComplete || playerInsideDetection || resetRoutine != null)
            {
                return;
            }

            resetRoutine = StartCoroutine(ResetAfterDelay());
        }

        private IEnumerator ResetAfterDelay()
        {
            yield return new WaitForSeconds(repeatResetDelay);

            if (!playerInsideDetection)
            {
                triggered = false;
                activationComplete = false;
                trackedPlayer = null;

                if (hideActivationVisualOnStart && activationVisual != null)
                {
                    activationVisual.SetActive(false);
                }
            }

            resetRoutine = null;
        }

        private void CancelPendingReset()
        {
            if (resetRoutine == null)
            {
                return;
            }

            StopCoroutine(resetRoutine);
            resetRoutine = null;
        }

        private void PlayActivationFeedback()
        {
            // 添加音效、动画
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

        private static bool TryGetPlayerController(Component other, out CharacterController playerController)
        {
            playerController = other.GetComponent<CharacterController>();
            if (playerController == null)
            {
                playerController = other.GetComponentInParent<CharacterController>();
            }

            return playerController != null &&
                   (other.CompareTag("Player") || playerController.CompareTag("Player"));
        }

        private static Vector3 Abs(Vector3 value)
        {
            return new Vector3(Mathf.Abs(value.x), Mathf.Abs(value.y), Mathf.Abs(value.z));
        }

        private void OnDrawGizmos()
        {
            Matrix4x4 previousMatrix = Gizmos.matrix;
            Color previousColor = Gizmos.color;
            Gizmos.matrix = transform.localToWorldMatrix;

            if (detectionZone != null && detectionZone.enabled)
            {
                DrawZone(detectionZone.center, detectionZone.size, new Color(1f, 0.15f, 0.05f, 0.16f));
            }
            else
            {
                DrawZone(detectionCenter, detectionSize, new Color(1f, 0.15f, 0.05f, 0.16f));
            }

            DrawZone(effectCenter, effectSize, new Color(1f, 0.75f, 0.05f, 0.14f));
            Gizmos.color = new Color(1f, 0.8f, 0.1f, 1f);
            Vector3 direction = impulseDirection.sqrMagnitude > 0.0001f ? impulseDirection.normalized : Vector3.forward;
            DrawImpulseArrow(effectCenter, direction, impulseDebugArrowLength);

            if (activationAnimationPoint != null)
            {
                Gizmos.matrix = Matrix4x4.identity;
                Gizmos.color = new Color(0.95f, 0.2f, 1f, 1f);
                Gizmos.DrawWireSphere(activationAnimationPoint.position, 0.3f);
                Gizmos.DrawLine(activationAnimationPoint.position, activationAnimationPoint.position + activationAnimationPoint.forward * 0.75f);
            }

            Gizmos.matrix = previousMatrix;
            Gizmos.color = previousColor;
        }

        private static void DrawZone(Vector3 center, Vector3 size, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawCube(center, size);
            Gizmos.color = new Color(color.r, color.g, color.b, 1f);
            Gizmos.DrawWireCube(center, size);
        }

        private static void DrawImpulseArrow(Vector3 origin, Vector3 direction, float length)
        {
            Vector3 end = origin + direction * length;
            Vector3 side = Vector3.Cross(direction, Vector3.up);
            if (side.sqrMagnitude < 0.001f)
            {
                side = Vector3.right;
            }

            side.Normalize();
            Vector3 arrowBase = end - direction * Mathf.Min(length * 0.22f, 0.65f);
            float arrowWidth = Mathf.Min(length * 0.1f, 0.35f);

            Gizmos.DrawLine(origin, end);
            Gizmos.DrawLine(end, arrowBase + side * arrowWidth);
            Gizmos.DrawLine(end, arrowBase - side * arrowWidth);
        }
    }
}
