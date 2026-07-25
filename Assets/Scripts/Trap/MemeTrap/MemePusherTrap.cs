using System.Collections;
using UnityEngine;

namespace CountdownTraps
{
    [RequireComponent(typeof(Collider))]
    public sealed class MemePusherTrap : MonoBehaviour
    {
        [Header("Meme")]
        [SerializeField] private GameObject meme;
        [SerializeField, Min(0.1f)] private float memeMoveSpeed = 12f;
        [SerializeField, Min(0.1f)] private float memeTravelDistance = 10f;

        [Header("Detection Zone")]
        [SerializeField] private BoxCollider detectionZone;
        [SerializeField] private Vector3 detectionCenter;
        [SerializeField] private Vector3 detectionSize = new Vector3(1f, 2f, 2f);

        private bool triggered;

        public BoxCollider DetectionZone => detectionZone;

        private void Reset()
        {
            detectionZone = GetComponent<BoxCollider>();

            ApplyDetectionZoneSettings();
        }

        private void Awake()
        {
            ApplyDetectionZoneSettings();

            if (Application.isPlaying && meme != null)
            {
                meme.SetActive(false);
            }
        }

        private void OnValidate()
        {
            ApplyDetectionZoneSettings();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (triggered || !TryGetPlayerController(other, out CharacterController playerController))
            {
                return;
            }

            triggered = true;
            StartCoroutine(PushPlayer(playerController));
        }

        private IEnumerator PushPlayer(CharacterController playerController)
        {
            if (meme == null)
            {
                yield break;
            }

            meme.SetActive(true);
            Vector3 memeTargetPosition = meme.transform.position + meme.transform.forward * memeTravelDistance;

            while (playerController != null &&
                   (meme.transform.position - memeTargetPosition).sqrMagnitude > 0.0001f)
            {
                Vector3 previousPosition = meme.transform.position;
                meme.transform.position = Vector3.MoveTowards(
                    previousPosition,
                    memeTargetPosition,
                    memeMoveSpeed * Time.deltaTime);

                Vector3 memeMoveDelta = meme.transform.position - previousPosition;
                if (MemeTouchesPlayer(playerController))
                {
                    // The player follows the physical meme only while the meme is in contact.
                    playerController.Move(memeMoveDelta);
                }

                yield return null;
            }
        }

        private bool MemeTouchesPlayer(CharacterController playerController)
        {
            Bounds playerBounds = playerController.bounds;

            foreach (Collider memeCollider in meme.GetComponentsInChildren<Collider>())
            {
                if (memeCollider.enabled && !memeCollider.isTrigger && memeCollider.bounds.Intersects(playerBounds))
                {
                    return true;
                }
            }

            return false;
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
    }
}
