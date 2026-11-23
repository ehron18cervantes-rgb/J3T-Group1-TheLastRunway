using UnityEngine;

public class CharacterSkinApplier : MonoBehaviour
{
    [Header("Order must match the Character Select buttons in MainMenu")]
    public RuntimeAnimatorController[] characterControllers;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        // Make sure every playable character/scene can shoot without requiring
        // manual Inspector changes. Existing PlayerShooting components are kept.
        if (GetComponent<PlayerShooting>() == null)
            gameObject.AddComponent<PlayerShooting>();
    }

    private void Start()
    {
        int selected = CharacterSelection.GetCharacter();

        // Character 3 is loaded from Resources so existing scenes and the first two
        // character assignments do not need to be edited.
        if (selected == 2)
        {
            RuntimeAnimatorController newCharacterController =
                Resources.Load<RuntimeAnimatorController>("NewCharacter/NewCharacter");

            if (newCharacterController != null)
            {
                animator.runtimeAnimatorController = newCharacterController;
                return;
            }

            Debug.LogWarning("[CharacterSkinApplier] New Character controller was not found in Resources.");
        }

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
