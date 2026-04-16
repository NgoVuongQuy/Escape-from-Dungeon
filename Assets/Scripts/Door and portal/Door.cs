using UnityEngine;
using TMPro;

public class Door : MonoBehaviour
{
    private Animator animator;
    private CapsuleCollider2D doorCollider;

    [SerializeField] private Transform player;
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private GameObject hintUI;

    private bool isOpen = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        doorCollider = GetComponent<CapsuleCollider2D>();
        hintUI.SetActive(false);
    }

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        float distance = Vector2.Distance(player.position, transform.position);

        if (distance <= interactionDistance)
        {
            hintUI.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                ToggleDoor();
            }
        }
        else
        {
            hintUI.SetActive(false);
        }
    }

    private void ToggleDoor()
    {
        isOpen = !isOpen;
        animator.SetBool("isOpen", isOpen);
        doorCollider.isTrigger = isOpen;
    }
}
