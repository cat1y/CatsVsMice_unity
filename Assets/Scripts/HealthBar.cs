using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Vector2 size = new Vector2(1f, 0.12f);
    [SerializeField] private float heightOffset = 0.2f; 
    private static readonly Color fillColor = new Color32(171, 255, 138, 255);
    private static readonly Color backColor = new Color32(159, 0, 71, 204);

    private Transform root;
    private Transform fill;
    private SpriteRenderer sr;
    private Enemy enemy;
    private Defender defender;
    private int maxHp;

    void Start()
    {
        enemy = GetComponent<Enemy>();
        defender = GetComponent<Defender>();
        sr = GetComponentInChildren<SpriteRenderer>();

        maxHp = enemy != null ? enemy.hp : (defender != null ? defender.hp : 1);

        Sprite square = MakeSquare();

        root = new GameObject("HealthBar").transform;
        GameObject back = new GameObject("Back");
        back.transform.SetParent(root, false);
        SpriteRenderer backRender = back.AddComponent<SpriteRenderer>();
        backRender.sprite = square;
        backRender.color = backColor;
        backRender.sortingOrder = sr.sortingOrder + 50;
        back.transform.localScale = new Vector3(size.x, size.y, 1f);

        GameObject front = new GameObject("Fill");
        front.transform.SetParent(root, false);
        fill = front.transform;
        SpriteRenderer fillRender = front.AddComponent<SpriteRenderer>();
        fillRender.sprite = square;
        fillRender.color = fillColor;
        fillRender.sortingOrder = sr.sortingOrder + 51;

        root.gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        if (root == null) return;
        if (Time.timeScale == 0f)
        {
            root.gameObject.SetActive(false);
            return;
        }

        int hp = enemy != null ? enemy.hp : defender.hp;
        bool dead = enemy != null ? enemy.IsDead : defender.IsDead;

        if (dead || hp >= maxHp)
        {
            root.gameObject.SetActive(false);
            return;
        }

        root.gameObject.SetActive(true);

        Bounds b = sr.bounds;
        root.position = new Vector3(b.center.x, b.max.y + heightOffset, 0f);

        float percent = Mathf.Clamp01((float)hp / maxHp);
        fill.localScale = new Vector3(size.x * percent, size.y, 1f);
        fill.localPosition = new Vector3(-size.x * (1f - percent) * 0.5f, 0f, 0f);
    }

    void OnDestroy()
    {
        if (root != null) Destroy(root.gameObject);
    }

    Sprite MakeSquare()
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        tex.filterMode = FilterMode.Point;
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }
}