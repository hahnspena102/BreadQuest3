using UnityEngine;

public class ItemAnimator : MonoBehaviour
{
    [SerializeField] private AnimationClip rangedChargePlaceholderClip;
    [SerializeField] private AnimationClip rangedReleasePlaceholderClip;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private RuntimeAnimatorController baseController;
    private AnimatorOverrideController overrideController;
    private AnimationClip currentRangedChargeClip;
    private AnimationClip currentRangedReleaseClip;
    private bool isRangedCharging;

    public Animator Animator => animator;

    public SpriteRenderer SpriteRenderer { get => spriteRenderer; set => spriteRenderer = value; }

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            baseController = animator.runtimeAnimatorController;
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void UseAnimation(string animationName)
    {
        if (animator != null)
        {
            animator.SetTrigger(animationName);
        }
    }

    void FinishItemAnimation()
    {
        UseItem useItem = GetComponentInParent<UseItem>();
        if (useItem != null)
        {
           useItem.FinishAttack();
        }
    }

    public void SetSpeed(float speed)
    {
        if (animator != null)
        {
            animator.speed = speed;
        }
    }

    public void BeginRangedCharge(RangedData rangedData)
    {
        if (rangedData == null || rangedData.ChargeAnimationClip == currentRangedChargeClip || rangedData.ReleaseAnimationClip == currentRangedReleaseClip)
        {
            return;
        }

        if (animator == null || rangedChargePlaceholderClip == null)
        {
            return;
        }

        RuntimeAnimatorController activeController = animator.runtimeAnimatorController;
        if (activeController == null)
        {
            return;
        }

        if (overrideController == null)
        {
            RuntimeAnimatorController resolvedBaseController = activeController;
            if (resolvedBaseController is AnimatorOverrideController existingOverride)
            {
                resolvedBaseController = existingOverride.runtimeAnimatorController;
            }

            baseController = resolvedBaseController;
            overrideController = new AnimatorOverrideController(baseController);


        }

        overrideController[rangedChargePlaceholderClip.name] = rangedData.ChargeAnimationClip;
        overrideController[rangedReleasePlaceholderClip.name] = rangedData.ReleaseAnimationClip;
        animator.runtimeAnimatorController = overrideController;
        animator.Rebind();
        animator.Update(0f);
        animator.speed = 1f / rangedData.ChargeTime;
        currentRangedChargeClip = rangedData.ChargeAnimationClip;
        isRangedCharging = true;
    }

    public void EndRangedCharge()
    {
        if (animator == null || baseController == null || !isRangedCharging)
        {
            return;
        }

        animator.runtimeAnimatorController = baseController;
        animator.Rebind();
        animator.Update(0f);
        isRangedCharging = false;
        currentRangedChargeClip = null;
        currentRangedReleaseClip = null;
    }
}
