using UnityEngine;

namespace CountdownTraps
{
    [DisallowMultipleComponent]
    public sealed class DebugReturnToSafety : MonoBehaviour
    {
#if UNITY_EDITOR
        private CharacterController characterController;
        private Vector3 safePosition;
        private Quaternion safeRotation;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            safePosition = transform.position;
            safeRotation = transform.rotation;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                ReturnToSafety();
            }
        }

        private void ReturnToSafety()
        {
            if (characterController != null)
            {
                characterController.enabled = false;
            }

            transform.SetPositionAndRotation(safePosition, safeRotation);

            if (characterController != null)
            {
                characterController.enabled = true;
            }
        }
#endif
    }
}
