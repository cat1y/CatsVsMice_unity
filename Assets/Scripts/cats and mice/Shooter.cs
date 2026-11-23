using UnityEngine;

public class Shooter : Defender
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireInterval = 1.5f;

    private Transform shootLimit;
    private FieldScript grid;
    private SpriteRenderer render;
    private float timer;
    private bool attacking;

    protected override void Awake()
    {
        base.Awake();
        GameObject limit = GameObject.Find("shootLimit");
        if (limit != null) shootLimit = limit.transform;
        grid = FindFirstObjectByType<FieldScript>();
        render = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (!Armed || IsDead) return;
        if (shootLimit == null || firePoint == null || bulletPrefab == null) return;

        bool seesEnemy = EnemyInLane();

        if (seesEnemy != attacking)
        {
            attacking = seesEnemy;
            if (anim != null) anim.SetBool("Attacking", attacking);
        }

        timer -= Time.deltaTime;
        if (!seesEnemy || timer > 0f) return;

        timer = fireInterval;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().damage = damage;

        AudioManager.Instance.Shoot();
    }

    bool EnemyInLane()
    {
        float catY = render.bounds.center.y;
        float halfRow = grid.CellSize.y * 0.5f;

        foreach (Enemy enemy in Enemy.All)
        {
            if (enemy.IsDead) continue;

            Vector2 p = enemy.Center;
            if (p.x < firePoint.position.x || p.x > shootLimit.position.x) continue;
            if (Mathf.Abs(p.y - catY) < halfRow) return true;
        }
        return false;
    }
}