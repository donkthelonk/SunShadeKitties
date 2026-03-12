using UnityEngine;

/// <summary>
/// Attach to any platform or object that should only be solid/active in one kitty state.
/// The collider is enabled when the player matches activeInState; otherwise it is disabled
/// and the sprite fades to indicate it is passable.
/// </summary>
public class StateObject : MonoBehaviour
{
    [SerializeField] private KittyState activeInState = KittyState.Sun;
    [SerializeField] private float inactiveAlpha = 0.25f;

    private Collider2D col;
    private SpriteRenderer sr;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        PlayerController.OnStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        PlayerController.OnStateChanged -= HandleStateChanged;
    }

    private void Start()
    {
        // Sync to whatever state the player starts in
        var player = FindFirstObjectByType<PlayerController>();
        if (player != null)
            ApplyState(player.State);
    }

    private void HandleStateChanged(KittyState state) => ApplyState(state);

    private void ApplyState(KittyState state)
    {
        bool active = state == activeInState;

        if (col != null) col.enabled = active;

        if (sr != null)
        {
            Color c = sr.color;
            c.a = active ? 1f : inactiveAlpha;
            sr.color = c;
        }
    }
}
