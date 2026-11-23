using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static User;

public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private int id;
    [SerializeField] private GameObject catPrefab;
    [SerializeField] private Text countText;
    [SerializeField] private FieldScript grid;

    private CatData cat;
    private int left;
    private GameObject ghost;

    void Start()
    {
        cat = PlayerData.GetCat(id);
        left = (cat != null && cat.isOwned) ? cat.limit : 0;

        Image image = GetComponent<Image>();
        if (image != null)
        {
            Color c = image.color;
            c.a = left > 0 ? 1f : 0.4f;
            image.color = c;
        }

        UpdateText();
    }

    void UpdateText()
    {
        if (countText != null) countText.text = left.ToString();
    }

    public void OnBeginDrag(PointerEventData e)
    {
        if (left <= 0) return;

        ghost = Instantiate(catPrefab, GetWorld(e), Quaternion.identity);

        Defender defender = ghost.GetComponent<Defender>();
        if (defender != null)
        {
            defender.hp = cat.health;
            defender.damage = cat.damage;
        }

        BombCat bombCat = ghost.GetComponent<BombCat>();
        if (bombCat != null) bombCat.damage = cat.damage;

        CenterOn(ghost, GetWorld(e));
    }

    public void OnDrag(PointerEventData e)
    {
        if (ghost != null) CenterOn(ghost, GetWorld(e));
    }

    public void OnEndDrag(PointerEventData e)
    {
        if (ghost == null) return;

        int col, row;
        Vector3 center;

        if (grid.TryGetCell(GetWorld(e), ghost, out col, out row, out center))
        {
            CenterOn(ghost, center);

            CatTank catTank = ghost.GetComponent<CatTank>();
            if (catTank != null) catTank.Arm();

            Defender defender = ghost.GetComponent<Defender>();
            if (defender != null) defender.Arm();

            BombCat bomb = ghost.GetComponent<BombCat>();
            if (bomb != null) bomb.Arm();

            left--;
            AudioManager.Instance.Place();
            UpdateText();
        }
        else
        {
            Destroy(ghost);
        }
        ghost = null;
    }

    void CenterOn(GameObject obj, Vector3 target)
    {
        SpriteRenderer sr = obj.GetComponentInChildren<SpriteRenderer>();
        Vector3 pivotToCenter = sr != null ? sr.bounds.center - obj.transform.position : Vector3.zero;
        Vector3 pos = target - pivotToCenter;
        pos.z = 0f;
        obj.transform.position = pos;
    }

    Vector3 GetWorld(PointerEventData e)
    {
        Vector3 p = Camera.main.ScreenToWorldPoint(e.position);
        p.z = 0f;
        return p;
    }
}