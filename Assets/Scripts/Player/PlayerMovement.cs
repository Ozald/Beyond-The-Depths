using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody2D rb2d;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public StatsManager stats;
    public bool canMove = true;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        stats = gameObject.GetComponent<StatsManager>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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

        rb2d.velocity = movementDirection * stats.speed.value;

        if (PlayerManager.instance.currentRoom is not null && PlayerManager.instance.currentRoom.hasBeenExplored)
            rb2d.velocity *= 1.5f;
        
        // rb2d.rotation = Vector2.SignedAngle(Vector2.up, movementDirection);



        Vector3 mousePos = Input.mousePosition;
        Vector3 viewportPos = new Vector3(mousePos.x / Screen.width, mousePos.y / Screen.height, 0);

        float distanceToWeapon = transform.position.z - Camera.main.transform.position.z;
        mousePos = Camera.main.ViewportToWorldPoint(new Vector3(viewportPos.x, viewportPos.y, distanceToWeapon));
        
        Vector3 mouseDir = (mousePos - transform.position).normalized;

        animator.SetFloat("X", mouseDir.x);
        animator.SetFloat("Y", mouseDir.y);

        if (mouseDir.x < 0.01f)
            spriteRenderer.flipX = true;
        else if (mouseDir.x > 0.01f)
            spriteRenderer.flipX = false;

        float speed = rb2d.velocity.magnitude;

        if (speed < 0.05f)
            speed = 0f;

        animator.SetFloat("Speed", speed);
    }


    
}
