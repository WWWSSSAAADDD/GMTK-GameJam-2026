using UnityEngine;

namespace CountdownTraps
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class TrapDetectionZoneGizmo : MonoBehaviour
    {
        [SerializeField] private BoxCollider detectionZone;
        [SerializeField] private Color gizmoColor = new Color(1f, 0.15f, 0.05f, 0.16f);

        private void OnDrawGizmos()
        {
            BoxCollider boxCollider = detectionZone != null ? detectionZone : FindTriggerCollider();
            if (boxCollider == null || !boxCollider.enabled)
            {
                return;
            }

            Matrix4x4 previousMatrix = Gizmos.matrix;
            Color previousColor = Gizmos.color;
            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.color = gizmoColor;
            Gizmos.DrawCube(boxCollider.center, boxCollider.size);

            Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 1f);
            Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);

            Gizmos.matrix = previousMatrix;
            Gizmos.color = previousColor;
        }

        private BoxCollider FindTriggerCollider()
        {
            foreach (BoxCollider boxCollider in GetComponents<BoxCollider>())
            {
                if (boxCollider.isTrigger)
                {
                    return boxCollider;
                }
            }

            return null;
        }
    }
}
