using UnityEngine;

public class CharacterSkinApplier : MonoBehaviour
{
    [Header("Order must match the Character Select buttons in MainMenu")]
    public RuntimeAnimatorController[] characterControllers;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        int selected = CharacterSelection.GetCharacter();

        if (characterControllers != null && selected >= 0 && selected < characterControllers.Length)
        {
            animator.runtimeAnimatorController = characterControllers[selected];
        }
        else
        {
            Debug.LogWarning($"[CharacterSkinApplier] No controller assigned for character index {selected}");
        }
    }
}