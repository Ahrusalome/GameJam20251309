using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 90;
    private Collider2D col;
    private Rigidbody2D rb;
    private Vector2 moveVector;
    private int collisionLayer;
    private GameObject collisionObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveVector.x, moveVector.y) * moveSpeed * Time.deltaTime;

        // transform.Translate(new Vector2(horizontalInput, verticalInput) * moveSpeed * Time.deltaTime);

    }
    public void OnInteract()
    {
        Debug.Log("Interact");
        switch (collisionLayer)
        {
            case (int)Layer.Door:
                if (collisionObject.GetComponent<Door>() != null)
                {
                    collisionObject.GetComponent<Door>().Open();
                }
                break;
            case (int)Layer.NPC:
                break;
            case (int)Layer.LookableObjects:
                if (collisionObject.GetComponentInChildren<Lookable>() != null)
                {
                    if (collisionObject.GetComponentInChildren<Lookable>().isVisible)
                        collisionObject.GetComponentInChildren<Lookable>().Disappear();
                    else collisionObject.GetComponentInChildren<Lookable>().Appear();
                }
                break;
            case (int)Layer.PickableObjects:
                // inventaire
                break;
        }

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collided");
        collisionLayer = collision.gameObject.layer;
        collisionObject = collision.gameObject;
    }
    public void OnTriggerExit2D()
    {
        Debug.Log("Exit");
        collisionLayer = 0;
        collisionObject = this.gameObject;
    }
    public void OnMove(InputValue value) => moveVector = value.Get<Vector2>();


}
