using System.Collections;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [Header("Trap Settings")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float activationDelay = 0.3f; // Thời gian trước khi gai dựng
    [SerializeField] private float activeTime = 1.5f; // Thời gian gai duy trì dựng
    [SerializeField] private float cooldownTime = 2f; // Thời gian nghỉ trước khi có thể kích hoạt lại
    
    private Animator animator;
    private bool isTriggered = false;
    private bool canDamage = false;
    private bool isOnCooldown = false;
    
    // Animation parameters - điều chỉnh theo Animator Controller của bạn
    private readonly string TRIGGER_PARAM = "Activate";
    private readonly string RESET_PARAM = "Reset";

    void Start()
    {
        animator = GetComponent<Animator>();
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra nếu là Player
        if (other.CompareTag("Player"))
        {
            // Nếu bẫy đang có thể gây damage (đang active)
            if (canDamage)
            {
                DamagePlayer();
            }
            // Nếu bẫy chưa kích hoạt và không trong cooldown
            else if (!isTriggered && !isOnCooldown)
            {
                StartCoroutine(ActivateTrapSequence());
            }
        }
    }
    
    void OnTriggerStay2D(Collider2D other)
    {
        // Tiếp tục gây sát thương nếu player đứng trên bẫy khi gai dựng
        if (other.CompareTag("Player") && canDamage)
        {
            DamagePlayer();
        }
    }
    
    private IEnumerator ActivateTrapSequence()
    {
        isTriggered = true;
        
        // Kích hoạt animation
        if (animator != null)
        {
            animator.SetTrigger(TRIGGER_PARAM);
        }
        
        // Đợi delay trước khi có thể gây sát thương
        yield return new WaitForSeconds(activationDelay);
        
        // Bây giờ có thể gây sát thương
        canDamage = true;
        
        // Kiểm tra nếu player vẫn đang trong trigger khi gai dựng lên
        Collider2D playerInTrap = Physics2D.OverlapBox(transform.position, 
            GetComponent<Collider2D>().bounds.size, 0f, LayerMask.GetMask("Player"));
        
        if (playerInTrap != null && playerInTrap.CompareTag("Player"))
        {
            DamagePlayer();
        }
        
        // Duy trì trạng thái active
        yield return new WaitForSeconds(activeTime);
        
        // Tắt khả năng gây sát thương
        canDamage = false;
        
        // Reset animation
        if (animator != null)
        {
            animator.SetTrigger(RESET_PARAM);
        }
        
        // Đợi animation reset hoàn thành
        yield return new WaitForSeconds(0.5f);
        
        // Reset trạng thái triggered
        isTriggered = false;
        
        // Bắt đầu cooldown
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        
        // Kết thúc cooldown - bẫy có thể kích hoạt lại
        isOnCooldown = false;
    }
    
    private void DamagePlayer()
    {
        if (PlayerHealth.Instance != null && !PlayerHealth.Instance.isDead)
        {
            PlayerHealth.Instance.TakeDamage(damage, transform);
        }
    }
    
    // Method để force reset bẫy (có thể dùng để debug)
    [ContextMenu("Force Reset Trap")]
    public void ForceReset()
    {
        StopAllCoroutines();
        isTriggered = false;
        canDamage = false;
        isOnCooldown = false;
        
        if (animator != null)
        {
            animator.SetTrigger(RESET_PARAM);
        }
    }
}