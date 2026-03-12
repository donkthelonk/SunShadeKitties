using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerVisuals : MonoBehaviour
{
    private SpriteRenderer sr;

    private void Awake()
    {
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
        // Apply the current state in case this enables after the player is already initialized
        sr.color = Color.white;
    }

    private void HandleStateChanged(KittyState state)
    {
        sr.color = state == KittyState.Sun ? Color.white : Color.black;
    }
}
