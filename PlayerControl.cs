using Assets.Scripts;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed;
    [SerializeField] private LevelGeneration levelGeneration;

    private Vector2Int curDirection;
    private GameManager gameManager;
    private UIManager uiManager;
    private bool isFacingRight = true;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        uiManager = FindObjectOfType<UIManager>();
    }

    public void Dammage(float value)
    {
        uiManager.ChangeHP(-value);
    }

    public void Healing(float value)
    {
        uiManager.ChangeHP(value);
    }

    public Vector2 GetPosition()
    {
        return rb.position;
    }

    //private float t;
    private void Update()
    {
        var position = new Vector2
        {
            x = Input.GetAxis("Horizontal") * speed * Time.deltaTime,
            y = Input.GetAxis("Vertical") * speed * Time.deltaTime
        };

        //if (t <= 0.5f)
        //{
        //    t += Time.deltaTime;
        //    return;
        //}

        //t = 0f;

        //var valueX = Input.GetAxis("Horizontal");
        //var valueY = Input.GetAxis("Vertical");

        //if (valueX == 0f && valueY == 0f)
        //{
        //    return;
        //}

        //var direction = new Vector2
        //{
        //    x = valueX,
        //    y = valueY
        //};

        if (!isFacingRight && position.x > 0)
        {
            Flip();
        }
        else if (isFacingRight && position.x < 0)
        {
            Flip();
        }

        //transform.localPosition = levelGeneration.GetNeighboringTilePosition(transform.localPosition, direction);
        rb.position += position;
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.right * 0.5f, Vector2.right, 2.5f);
            Debug.DrawRay(transform.position, Vector2.right * 2.5f, Color.red);

            if (hit.collider != null && hit.collider.CompareTag("Enemy"))
            {
                hit.transform.GetComponent<Enemy>().Damage();
            }
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        
        transform.localScale = Scaler;
    }

    public void PlaceInCenter()
    {
        rb.position = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.CompareTag("Door"))
        {
            gameManager?.StartGame();
        }
    }
}
