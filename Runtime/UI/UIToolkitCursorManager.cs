using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUtilities.UI
{

    [RequireComponent(typeof(UIDocument))]
    public class UIToolkitCursorManager : MonoBehaviour
    {
        public static bool IsCursorOverUI
        {
            get; protected set;
        }
        private void OnEnable()
        {
            var d = gameObject.GetComponent<UIDocument>();
            d.rootVisualElement.Query(className: "blocking-raycast").ForEach(v =>
            {
                v.RegisterCallback<PointerEnterEvent>(_ =>
                {
                    IsCursorOverUI = true;
                });
                v.RegisterCallback<PointerLeaveEvent>(_ =>
                {
                    IsCursorOverUI = false;
                });
            });
        }
        private void OnDisable()
        {
            var d = gameObject.GetComponent<UIDocument>();
            d.rootVisualElement.Query(className: "blocking-raycast").ForEach(v =>
            {
                v.UnregisterCallback<PointerEnterEvent>(OnPointerEnter);
                v.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
            });
        }
        private void OnPointerEnter(object _)
        {
            IsCursorOverUI = true;
        }
        private void OnPointerLeave(object _)
        {
            IsCursorOverUI = false;
        }
    }
}