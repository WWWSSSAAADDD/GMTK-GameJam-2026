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

        private bool triggered;

        private void Awake()
        {
            SetCageActive(false);
        }

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (triggered || !IsPlayer(other))
            {
                return;
            }

            triggered = true;
            SetCageActive(true);
            StartCoroutine(RemoveFloor());
        }

        private IEnumerator RemoveFloor()
        {
            yield return new WaitForSeconds(dropDelay);

            if (floorToRemove != null)
            {
                floorToRemove.SetActive(false);
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
