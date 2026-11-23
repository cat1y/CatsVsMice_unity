using UnityEngine;

public class FieldScript : MonoBehaviour
{
    [SerializeField] private RectTransform topLeft;
    [SerializeField] private RectTransform bottomRight;
    [SerializeField] private int columns = 8;
    [SerializeField] private int rows = 4;

    private bool[,] busy;

    void Awake()
    {
        busy = new bool[columns, rows];
    }

    public bool TryGetCell(Vector3 worldPos, GameObject ignore, out int col, out int row, out Vector3 center)
    {
        Vector3 a = topLeft.position;
        Vector3 b = bottomRight.position;

        float cellW = (b.x - a.x) / columns;
        float cellH = (a.y - b.y) / rows;

        col = Mathf.FloorToInt((worldPos.x - a.x) / cellW);
        row = Mathf.FloorToInt((a.y - worldPos.y) / cellH);

        center = new Vector3(a.x + (col + 0.5f) * cellW, a.y - (row + 0.5f) * cellH, 0f);

        if (col < 0 || col >= columns || row < 0 || row >= rows) return false;

        foreach (Collider2D c in Physics2D.OverlapPointAll(center))
        {
            if (c.transform.IsChildOf(ignore.transform)) continue;

            if (c.GetComponentInParent<Defender>() != null) return false;
            if (c.GetComponentInParent<BombCat>() != null) return false;
            if (c.GetComponentInParent<CatTank>() != null) return false;
        }

        return true;
    }

    public void Occupy(int col, int row)
    {
        busy[col, row] = true;
    }

    void OnDrawGizmos()
    {
        if (topLeft == null || bottomRight == null) return;

        Vector3 a = topLeft.position;
        Vector3 b = bottomRight.position;
        float cellW = (b.x - a.x) / columns;
        float cellH = (a.y - b.y) / rows;

        Gizmos.color = Color.red;
        for (int c = 0; c <= columns; c++)
        {
            float x = a.x + c * cellW;
            Gizmos.DrawLine(new Vector3(x, a.y, 0f), new Vector3(x, b.y, 0f));
        }
        for (int r = 0; r <= rows; r++)
        {
            float y = a.y - r * cellH;
            Gizmos.DrawLine(new Vector3(a.x, y, 0f), new Vector3(b.x, y, 0f));
        }
    }

    public Vector2 CellSize
    {
        get
        {
            Vector3 a = topLeft.position;
            Vector3 b = bottomRight.position;
            return new Vector2((b.x - a.x) / columns, (a.y - b.y) / rows);
        }
    }
}