using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody2D rb2d;
    public PlayerSettings PS;
    public bool canMove = true;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove)
        {
            rb2d.velocity = Vector2.zero;
            return;
        }

        double horizontalInput = Input.GetAxisRaw("Horizontal");
        // Move the player based on the input (X-axis)

        double verticalInput = Input.GetAxisRaw("Vertical");
        // Move the player based on the input (Y-axis)

        // Debug.Log("Horizontal Input: " + horizontalInput + " |" + " Vertical Input: " + verticalInput);

        Vector2 movementDirection = new Vector2((float)horizontalInput, (float)verticalInput).normalized;

        rb2d.velocity = movementDirection * PS.speedVariable;
        rb2d.rotation = Vector2.SignedAngle(Vector2.up, movementDirection);
    }
}
