using System.Collections;
using UnityEngine;

public class Defender : MonoBehaviour
{
    public int hp = 5; 
    public int damage = 1;
    public float deathDelay = 0.8f;
    public float hitTime = 0.15f;
    public float shakeStrength = 0.05f;

    public bool Armed { get; private set; }
    public bool IsDead { get; private set; }

    protected Animator anim;

    private SpriteRenderer sr;
    private Coroutine hitRoutine;
    private Vector3 shakeBase;
    private bool shaking;

   

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
        if (anim != null) anim.speed = 0f;
    }

    public void Arm()
    {
        Armed = true;
        if (anim != null) anim.speed = 1f;
    }
    public void TakeDamage(int damage)
    {
        if (IsDead) return;

        hp -= damage;
        AudioManager.Instance.Hit();

        if (hp <= 0)
        {
            IsDead = true;
            StopHitEffect();

            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            SetTriggerSafe("Die");
            Destroy(gameObject, deathDelay);
            return;
        }

        StopHitEffect();
        hitRoutine = StartCoroutine(HitEffect());
    }

    IEnumerator HitEffect()
    {
        shakeBase = sr.transform.localPosition;
        shaking = true;
        sr.color = new Color(1f, 0.55f, 0.55f);

        float t = 0f;
        while (t < hitTime)
        {
            sr.transform.localPosition = shakeBase + (Vector3)(Random.insideUnitCircle * shakeStrength);
            t += Time.deltaTime;
            yield return null;
        }

        StopHitEffect();
    }

    void StopHitEffect()
    {
        if (hitRoutine != null)
        {
            StopCoroutine(hitRoutine);
            hitRoutine = null;
        }

        if (shaking)
        {
            sr.transform.localPosition = shakeBase;
            shaking = false;
        }

        if (sr != null) sr.color = Color.white;
    }

    protected void SetTriggerSafe(string name)
    {
        if (anim == null || !anim.enabled || anim.runtimeAnimatorController == null) return;

        foreach (AnimatorControllerParameter p in anim.parameters)
        {
            if (p.name == name)
            {
                anim.SetTrigger(name);
                return;
            }
        }
    }
}