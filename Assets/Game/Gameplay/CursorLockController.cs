using UnityEngine;
using UnityEngine.InputSystem;

namespace Entropa.Gameplay
{
    public sealed class CursorLockController : MonoBehaviour
    {
        [SerializeField] private bool _lockOnStart = true;
        [SerializeField] private bool _lockOnClick = true;

        private bool _locked;

        private void Start()
        {
            if (_lockOnStart && !Application.isEditor)
            {
                Lock();
            }
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Unlock();
                return;
            }

            if (_lockOnClick && !_locked && WasClickInsideGameView())
            {
                Lock();
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                Unlock();
            }
        }

        private void OnDisable()
        {
            Unlock();
        }

        private static bool WasClickInsideGameView()
        {
            if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
            {
                return false;
            }

#if UNITY_EDITOR
            UnityEditor.EditorWindow hovered = UnityEditor.EditorWindow.mouseOverWindow;
            return hovered != null && hovered.GetType().Name == "GameView";
#else
            return true;
#endif
        }

        public void Lock()
        {
            _locked = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void Unlock()
        {
            _locked = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
