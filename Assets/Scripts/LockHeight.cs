using UnityEngine;

public class LockHeight : MonoBehaviour
{
    public float lockedY = 0f; // Set this to your road height (e.g., 0 or 1.3)

    void LateUpdate()
    {
        // Force the Y position to stay the same, but let X and Z move
        Vector3 pos = transform.position;
        pos.y = lockedY;
        transform.position = pos;
    }
}