using UnityEngine;

public class Scarecrow : Defender
{
    [SerializeField] private Sprite[] stages;

    private int maxHp;
    private SpriteRenderer render;
    private int currentStage = -1;

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        maxHp = hp;
        render = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (!Armed || IsDead || stages.Length == 0) return;

        float percent = (float)hp / maxHp;
        int stage = Mathf.Clamp(Mathf.FloorToInt((1f - percent) * stages.Length), 0, stages.Length - 1);

        if (stage == currentStage) return;
        currentStage = stage;

        if (stage > 0 && anim != null) anim.enabled = false;
        if (stage > 0) render.sprite = stages[stage];
    }
}