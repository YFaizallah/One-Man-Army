using UnityEngine;

public class CutsceneWalker : MonoBehaviour
{
    public float speed = 1.6f;
    public float rayHeight = 2f;
    public LayerMask groundMask;

    private bool groundedOnce = false;
    private float yOffsetFromGround;

    void LateUpdate()
    {
        // Move forward visually
        transform.position += transform.forward * speed * Time.deltaTime;

        Ray ray = new Ray(transform.position + Vector3.up * rayHeight, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, rayHeight * 2f, groundMask))
        {
            if (!groundedOnce)
            {
                // Cache initial offset (prevents jump)
                yOffsetFromGround = transform.position.y - hit.point.y;
                groundedOnce = true;
            }

            Vector3 pos = transform.position;
            pos.y = hit.point.y + yOffsetFromGround;
            transform.position = pos;
        }
    }
}
