using System;
using UnityEngine.UIElements;

namespace Screens
{
    public static class CommonBinds
    {
        public static void BindButtons(this VisualElement element, params (string id, Action action)[] bindings)
        {
            var buttons = new (Button element, Action action)[bindings.Length];
            for (var i = 0; i < bindings.Length; i++)
            {
                var (id, action) = bindings[i];
                buttons[i] = (element.Q<Button>(id), () => action());
            }

            element.RegisterCallback<AttachToPanelEvent>(AttachObservers);
            element.RegisterCallback<DetachFromPanelEvent>(DetachObservers);

            return;

            void AttachObservers(AttachToPanelEvent _)
            {
                foreach (var (item, action) in buttons)
                {
                    item.clicked += action;
                }
            }

            void DetachObservers(DetachFromPanelEvent _)
            {
                foreach (var (item, action) in buttons)
                {
                    item.clicked -= action;
                }
            }
        }
    }
}