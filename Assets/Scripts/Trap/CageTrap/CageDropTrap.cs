using System.Collections;
using UnityEngine;

namespace CountdownTraps
{
    [RequireComponent(typeof(Collider))]
    public sealed class CageDropTrap : MonoBehaviour
    {
        [SerializeField] private GameObject floorToRemove;
        [SerializeField] private GameObject cageWest;
        [SerializeField] private GameObject cageEast;
        [SerializeField] private GameObject cageSouth;
        [SerializeField] private GameObject cageNorth;
        [SerializeField] private GameObject cageRoof;
        [SerializeField, Min(0f)] private float dropDelay = 0.25f;

        [Header("Trigger")]
        [SerializeField] private bool triggerOnce = true;
        [SerializeField, Min(0f)] private float repeatResetDelay = 2f;

        [Header("Audio")]
        [SerializeField] private TrapTriggerAudio triggerAudio = new TrapTriggerAudio();

        private bool triggered;
        private bool playerInside;
        private bool floorRemoved;
        private Coroutine resetRoutine;

        private void Awake()
        {
            SetCageActive(false);
            SetActive(floorToRemove, true);
            floorRemoved = false;
        }

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
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
            SetCageActive(true);
            triggerAudio.Play(GetComponent<AudioSource>());
            StartCoroutine(RemoveFloor());
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

        private IEnumerator RemoveFloor()
        {
            yield return new WaitForSeconds(dropDelay);

            if (floorToRemove != null)
            {
                floorToRemove.SetActive(false);
            }

            floorRemoved = true;
        }

        private IEnumerator ResetAfterPlayerLeaves()
        {
            while (!floorRemoved || playerInside)
            {
                yield return null;
            }

            yield return new WaitForSeconds(repeatResetDelay);

            if (!playerInside)
            {
                SetCageActive(false);
                SetActive(floorToRemove, true);
                floorRemoved = false;
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

        private void SetCageActive(bool active)
        {
            SetActive(cageWest, active);
            SetActive(cageEast, active);
            SetActive(cageSouth, active);
            SetActive(cageNorth, active);
            SetActive(cageRoof, active);
        }

        private static void SetActive(GameObject gameObject, bool active)
        {
            if (gameObject != null)
            {
                gameObject.SetActive(active);
            }
        }

        private static bool IsPlayer(Component other)
        {
            return other.CompareTag("Player") || other.GetComponentInParent<CharacterController>() != null;
        }
    }
}
