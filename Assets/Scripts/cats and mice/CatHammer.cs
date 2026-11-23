using System.Collections;
using UnityEngine;

public class CatHammer1 : Defender
{
    [SerializeField] private float attackInterval = 1f;
    [SerializeField] private float hitDelay = 0.3f;
    [SerializeField] private float rangeInCells = 1f;

    private FieldScript grid;
    private SpriteRenderer render;
    private float timer;
    private bool attacking;

    protected override void Awake()
    {
        base.Awake();
        grid = FindFirstObjectByType<FieldScript>();
        render = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (!Armed || IsDead) return;

        Enemy target = FindTarget();
        bool seesEnemy = target != null;

        if (seesEnemy != attacking)
        {
            attacking = seesEnemy;
            if (anim != null) anim.SetBool("Attacking", attacking);
        }

        timer -= Time.deltaTime;
        if (!seesEnemy || timer > 0f) return;

        timer = attackInterval;
        StartCoroutine(Hit(target));
    }

    IEnumerator Hit(Enemy target)
    {
        yield return new WaitForSeconds(hitDelay);
        if (IsDead || target == null) yield break;
        if (FindTarget() == target) target.TakeDamage(damage);
    }

    Enemy FindTarget()
    {
        Vector2 start = render.bounds.center;
        float length = grid.CellSize.x * rangeInCells + render.bounds.extents.x;

        Enemy nearest = null;
        float best = float.MaxValue;

        foreach (RaycastHit2D hit in Physics2D.RaycastAll(start, Vector2.right, length))
        {
            Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
            if (enemy != null && hit.distance < best)
            {
                best = hit.distance;
                nearest = enemy;
            }
        }
        return nearest;
    }
}