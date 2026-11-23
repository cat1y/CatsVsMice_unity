using System.Collections;
using UnityEngine;

public class BombCat : MonoBehaviour
{
    public int damage = 10;
    [SerializeField] private float damageDelay = 0.3f;
    [SerializeField] private float destroyDelay = 1f;

    private Animator anim;
    private SpriteRenderer sr;
    private FieldScript grid;
    private bool armed;
    private bool exploded;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
        grid = FindFirstObjectByType<FieldScript>();
        if (anim != null) anim.speed = 0f;
    }

    public void Arm()
    {
        armed = true;
        if (anim != null) anim.speed = 1f;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!armed || exploded) return;
        if (other.GetComponentInParent<Enemy>() == null) return;

        exploded = true;
        StartCoroutine(Explode());
    }

    IEnumerator Explode()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (anim != null) anim.SetTrigger("Explode");

        yield return new WaitForSeconds(damageDelay);
        AudioManager.Instance.Explosion();

        Vector2 cell = grid.CellSize;
        Vector2 center = sr.bounds.center;
        Vector2 size = new Vector2(cell.x * 3f, cell.y * 3f);

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f);
        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponentInParent<Enemy>();
            if (enemy == null) continue;

            Vector2 p = hit.bounds.center;
            if (Mathf.Abs(p.x - center.x) > size.x / 2f) continue;
            if (Mathf.Abs(p.y - center.y) > size.y / 2f) continue;

            enemy.TakeDamage(damage);
        }

        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}