using Assets.Scripts;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float distance;
    private MovePosition movePosition;
    private Coroutine movingCor, moveToCor;
    private PlayerControl player;
    private bool isAttack;

    public void Init(MovePosition positions, PlayerControl playerControl)
    {
        movePosition = positions;
        player = playerControl;
        movingCor = StartCoroutine(Moving());
    }

    public void Damage()
    {
        Destroy(gameObject);
    }

    private Direction GetRandomDirection()
    {
        var r = Random.Range(0, 2);
        Direction dir = null;
        switch (r)
        {
            //case 0:
            //    dir = movePosition.Right;
            //    if (dir.isAvailable) return dir;
            //    break;
            case 0:
                dir = movePosition.Left;
                if (dir.isAvailable) return dir;
                break;
            //case 2:
            //    dir = movePosition.Up;
            //    if (dir.isAvailable) return dir;
            //    break;
            case 1:
                dir = movePosition.Down;
                if (dir.isAvailable) return dir;
                break;
        }

        return dir;
    }

    private IEnumerator Moving()
    {
        while (true)
        {
            int i = 0;
            Direction dir = null;

            do
            {
                dir = GetRandomDirection();
                yield return null;
            }
            while (dir == null && ++i < 50);

            if (dir == null)
            {
                yield return new WaitForSeconds(1.5f);
                continue;
            }

            while ((Vector2)transform.localPosition != dir.Position)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, dir.Position, 1.5f * Time.deltaTime);
                yield return null;
            }

            yield return new WaitForSeconds(1f);
            var center = movePosition.Center.Position;

            while ((Vector2)transform.localPosition != center)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, center, 1.5f * Time.deltaTime);
                yield return null;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator MoveToCor(Vector3 targetPosition)
    {
        while (transform.position != targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, 1.5f * Time.deltaTime);
            yield return null;
        }

        moveToCor = null;
    }

    private void MoveTo(Vector3 targetPosition)
    {
        if(moveToCor != null) StopCoroutine(moveToCor);
        moveToCor = StartCoroutine(MoveToCor(targetPosition));
    }

    public void Attack()
    {
        if (movingCor != null)
        {
            StopCoroutine(movingCor);
            movingCor = null;
            player.Dammage(10f);
        }

        MoveTo(player.transform.position);
    }

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        var dis = Vector3.Distance(transform.position, player.transform.position);
        if (dis <= distance)
        {
            isAttack = true;
            Attack();
        }
        
        //if (isAttack)
        //{
        //    isAttack = false;
        //    movingCor = StartCoroutine(Moving());
        //}
    }
}
