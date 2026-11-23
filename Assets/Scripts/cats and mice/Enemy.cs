using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 0.5f;
    public int hp = 3;
    public int damage = 1;
    public float attackInterval = 1f;
    public float deathDelay = 0.7f;

    public static int AliveCount;

    private bool dead;
    private bool attacking;
    private Defender target;
    private float timer;
    private Animator anim;
    public float minGap = 0.15f;

    private FieldScript grid;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        grid = FindFirstObjectByType<FieldScript>();
    }

    bool BlockedByEnemy()
    {
        Collider2D mine = GetComponentInChildren<Collider2D>();
        if (mine == null || grid == null) return false;

        float halfRow = grid.CellSize.y * 0.5f;

        foreach (Enemy other in All)
        {
            if (other == this || other.IsDead) continue;

            Collider2D theirs = other.GetComponentInChildren<Collider2D>();
            if (theirs == null) continue;

            if (Mathf.Abs(other.Center.y - Center.y) > halfRow) continue;
            if (other.Center.x >= Center.x) continue;

            float gap = mine.bounds.min.x - theirs.bounds.max.x;
            if (gap < minGap) return true;
        }
        return false;
    }


    public static readonly System.Collections.Generic.List<Enemy> All = new System.Collections.Generic.List<Enemy>();

    public bool IsDead { get { return dead; } }

    public Vector2 Center
    {
        get
        {
            Collider2D c = GetComponentInChildren<Collider2D>();
            return c != null ? (Vector2)c.bounds.center : (Vector2)transform.position;
        }
    }

    void OnEnable()
    {
        AliveCount++;
        All.Add(this);
    }

    void OnDisable()
    {
        AliveCount--;
        All.Remove(this);
    }


    void Update()
    {
        if (dead) return;

        if (target != null && !target.IsDead)
        {
            SetAttacking(true);

            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                timer = attackInterval;
                target.TakeDamage(damage);
            }
            return;
        }

        target = null;
        SetAttacking(false);
        target = null;
        SetAttacking(false);

        if (BlockedByEnemy()) return;

        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (dead || target != null) return;

        Defender d = other.GetComponentInParent<Defender>();
        if (d == null || !d.Armed || d.IsDead) return;

        FieldScript grid = FindFirstObjectByType<FieldScript>();
        float rowHeight = grid.CellSize.y;

        SpriteRenderer defenderSprite = d.GetComponentInChildren<SpriteRenderer>();
        if (Mathf.Abs(Center.y - defenderSprite.bounds.center.y) > rowHeight * 0.5f) return;

        target = d;
        timer = attackInterval;
    }

    void SetAttacking(bool value)
    {
        if (attacking == value) return;
        attacking = value;
        if (anim != null) anim.SetBool("Attacking", value);
    }

    public void TakeDamage(int amount)
    {
        if (dead) return;

        hp -= amount;
        AudioManager.Instance.Hit();
        if (hp <= 0)
        {
            Die();
            return;
        }

        if (anim != null) anim.SetTrigger("Hit");
    }


    public void Die()
    {
        if (dead) return;
        dead = true;
        target = null;
        enabled = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (anim != null) anim.SetTrigger("Die");
        Destroy(gameObject, deathDelay);
    }
}