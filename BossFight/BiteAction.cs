using System;
using System.Collections;
using UnityEngine;

public class BiteAction : Action
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float duration = 1f;
    [SerializeField] private AnimationCurve curve;
    [Space]
    [SerializeField] private Transform target;
    [SerializeField] private float maxAttackRange = 10;

    private Animator animator;

    private bool isAnimating;
    private bool isDone;
    private bool hasAttacked;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public override bool CanPerform()
    {
        var projectile = FindObjectOfType<Projectile>();
        var inRange = Vector3.Distance(transform.position, target.position) > maxAttackRange;
        
        EventSystem.AddListener(EventType.BOSS_DAMAGED, OnBossDamaged);

        return projectile == null && inRange;
    }

    public override bool PerformAction()
    {
        if (isDone) return true;

        if (!isAnimating)
        {
            animator.SetTrigger("attack");
            StartCoroutine(Attack(transform.position, target.position));
        }

        return false;
    }
    
    protected override void Reset()
    {
        isDone = false;
        EventSystem.RemoveListener(EventType.BOSS_DAMAGED, OnBossDamaged);
    }

    private void OnBossDamaged()
    {
        hasAttacked = true;
    }
    
    private IEnumerator Attack(Vector3 origin, Vector3 targetPos)
    {
        isAnimating = true;
        hasAttacked = false;
        
        var journey = 0f;
        while (journey <= duration)
        {
            journey += Time.deltaTime;
            var percent = Mathf.Clamp01(journey / duration);
            
            var curvePercent = curve.Evaluate(percent);
            transform.position = Vector3.LerpUnclamped(origin, targetPos, curvePercent);

            if (!hasAttacked)
            {
                if (Math.Abs(curve.Evaluate(percent) - 1) < 0.05f)
                {
                    EventSystem<int>.InvokeEvent(EventType.DAMAGE_PLAYER, damage);
                    
                    hasAttacked = true;
                }
            }

            yield return null;
        }

        isDone = true;
        isAnimating = false;
    }
}