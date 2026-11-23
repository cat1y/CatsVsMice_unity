using UnityEngine;

public class CatTank : MonoBehaviour
{
    [SerializeField] private float speed = 6f;
    [SerializeField] private float maxX = 12f;
    [SerializeField] private bool armed;

    private bool running;
    private Animator anim;
    void Start()
    {
        Arm();
    }
    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void Arm()
    {
        armed = true;
    }

    void Update()
    {
        if (!running) return;

        transform.Translate(Vector3.right * speed * Time.deltaTime);

        if (transform.position.x > maxX)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!armed) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null) return;

        if (!running)
        {
            running = true;
            if (anim != null) anim.SetTrigger("Run");
            AudioManager.Instance.Tank();
        }

        enemy.Die();
    }
}