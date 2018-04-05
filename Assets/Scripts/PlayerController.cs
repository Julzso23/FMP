using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private new Rigidbody2D rigidbody;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpSpeed;
    private bool grounded = false;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Vector2 newVelocity = new Vector2(Input.GetAxis("Horizontal") * moveSpeed, rigidbody.velocity.y);

        if (Input.GetButton("Jump") && grounded)
        {
            newVelocity.y = jumpSpeed;
        }

        rigidbody.velocity = newVelocity;

        grounded = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D point in collision.contacts)
        {
            if ((Mathf.Abs(point.normal.x) <= 0.5f) && (point.normal.y > 0f))
            {
                grounded = true;
            }
        }
    }
}
