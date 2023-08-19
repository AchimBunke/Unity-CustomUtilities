using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUtilities.UI.CustomControls
{
    /// <summary>
    /// Maybe use in the future
    /// </summary>
    public class TabControl : VisualElement
    {
        [UnityEngine.Scripting.Preserve]
        public new class UxmlFactory : UxmlFactory<TabControl, UxmlTraits> { }

        public enum TabControlPosition
        {
            Top, Right, Bottom, Left
        }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlEnumAttributeDescription<TabControlPosition> m_position =
                new UxmlEnumAttributeDescription<TabControlPosition> { name = "tab-position", defaultValue = TabControlPosition.Top };
            

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var ate = ve as TabControl;

                ate.tabposition = m_position.GetValueFromBag(bag, cc);
            }
        }

        private TabControlPosition _tabPosition;
        public TabControlPosition tabposition 
        {
            get => _tabPosition;
            set { _tabPosition = value; TabPositionChanged(); }
        }
        private void TabPositionChanged()
        {
            switch (_tabPosition)
            {
                case TabControlPosition.Top:
                    {
                        this.style.flexDirection = FlexDirection.Column; 
                        tabPanel.style.flexDirection = FlexDirection.Row;
                        break;
                    }
                case TabControlPosition.Right:
                    {
                        this.style.flexDirection = FlexDirection.RowReverse;
                        tabPanel.style.flexDirection = FlexDirection.Column;
                        break;
                    }
                case TabControlPosition.Bottom:
                    {
                        this.style.flexDirection = FlexDirection.ColumnReverse;
                        tabPanel.style.flexDirection = FlexDirection.Row;
                        break;
                    }
                case TabControlPosition.Left:
                    {
                        this.style.flexDirection = FlexDirection.Row;
                        tabPanel.style.flexDirection = FlexDirection.Column;
                        break;
                    }
            }
        }

        VisualElement tabPanel;
        VisualElement contentPanel;
        public TabControl()
        {
            tabPanel = new VisualElement();
            hierarchy.Add(tabPanel);
            contentPanel = new VisualElement();
            hierarchy.Add(contentPanel);
        }

        private List<TabItem> items = new List<TabItem>();
    }
}
