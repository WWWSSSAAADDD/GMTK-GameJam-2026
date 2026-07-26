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

        [Header("Detection Zone")]
        [SerializeField] private BoxCollider detectionZone;
        [SerializeField] private Vector3 detectionCenter;
        [SerializeField] private Vector3 detectionSize = new Vector3(1f, 2f, 2f);

        private bool triggered;

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
            ApplyDetectionZoneSettings();
        }

        private void OnValidate()
        {
            ApplyDetectionZoneSettings();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (triggered || !IsPlayer(other))
            {
                return;
            }

            triggered = true;
            StartCoroutine(MovePlatform());
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
