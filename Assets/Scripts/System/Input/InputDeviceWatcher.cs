using EditorTools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public sealed class InputDeviceWatcher : MonoBehaviour
{
    public static InputDeviceWatcher Instance { get; private set; }
    [HideInInspector]
    public PlayerInput playerInput;
    [ShowOnly]
    [SerializeField]
    private string currentControlScheme;
    public static UnityEvent<PlayerInput> onControlsChanged = new();

    private void Awake()
    {
        Instance = this;
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (playerInput.currentControlScheme != currentControlScheme)
        {
            OnControlsChanged(playerInput);
            currentControlScheme = playerInput.currentControlScheme;
        }
    }

    private void OnControlsChanged(PlayerInput playerInput)
    {
        Debug.Log($"Active scheme: {playerInput.currentControlScheme}");
        onControlsChanged.Invoke(playerInput);
    }

    private void OnEnable() { playerInput.onControlsChanged += OnControlsChanged; }
    private void OnDisable() { playerInput.onControlsChanged -= OnControlsChanged; }
}