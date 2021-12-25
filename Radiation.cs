using UnityEngine;

public class Radiation : MonoBehaviour
{
    [SerializeField] private float radius = 1;
    [SerializeField] private float dammagePower = 1;
    [SerializeField] private float pauseTime = 1;
    private PlayerControl player;
    private float time;

    private void Start()
    {
        player = FindObjectOfType<PlayerControl>();
        time = pauseTime;
    }

    private void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) <= radius)
        {
            var direction = player.transform.position - transform.position;
            var hit = Physics2D.Raycast(transform.position, direction);
            if (hit && hit.transform.CompareTag("Wall"))
            {
                return;
            }

            if (time >= 0f)
            {
                time -= Time.deltaTime;
                return;
            }

            if (Vector3.Distance(player.transform.position, transform.position) < 0.1f)
            {
                player.Dammage(10);
            }
            else
            {
                player.Dammage(1 * (radius / Vector3.Distance(player.transform.position, transform.position)));
            }
            time = pauseTime;
        }
    }
}
