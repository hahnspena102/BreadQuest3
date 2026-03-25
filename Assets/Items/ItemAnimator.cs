using UnityEngine;

public class ItemAnimator : MonoBehaviour
{
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
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
}
