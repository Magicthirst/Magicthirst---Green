using System;
using System.Linq;
using UnityEngine.InputSystem;

namespace Levels
{
    internal static class PlayerInputExtension
    {
        public static GenericAction<T> ConsumeAction<T>(this InputActionMap map, string actionName) where T : struct => new(map, actionName);

        public class GenericAction<T> : IDisposable where T : struct
        {
            public event Action<T> Performed;

            private readonly InputAction _action;

            public GenericAction(InputActionMap map, string actionName)
            {
                try
                {
                    _action = map.FindAction(actionName, true);
                    _action.Enable();
                }
                catch (ArgumentException e)
                {
                    throw new ArgumentException
                    (
                        "Existing action names: " + string.Join(", ", map.actions.Select(a => a.name).ToArray()),
                        e
                    );
                }
                _action.performed += OnActionPerformed;
                _action.canceled += OnActionPerformed;
            }

            public void Dispose()
            {
                _action.performed -= OnActionPerformed;
            }

            public GenericAction<T> OnPerformed(Action<T> action)
            {
                Performed += action;
                return this;
            }

            private void OnActionPerformed(InputAction.CallbackContext context) => Performed?.Invoke(context.ReadValue<T>());
        }
    }
}
