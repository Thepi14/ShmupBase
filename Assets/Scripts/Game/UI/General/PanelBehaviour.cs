using System.Collections.Generic;
using EditorTools;
using UnityEngine;
using UnityEngine.UI;

namespace Main.UI
{
    public abstract class PanelBehaviour : MonoBehaviour
    {
        public static List<PanelBehaviour> panels = new();
        public static PanelBehaviour currentPanel, previousPanel;
        protected Image background;

        [ShowOnly]
        public bool opened = false;
        public bool main = false;

        protected virtual void Awake()
        {
            panels ??= new();
            background = GetComponent<Image>();

            AddThisPanel();
        }

        protected virtual void Start()
        {
            if (panels.Count == 1)
                main = true;

            ReturnToMain(true);
        }

        public virtual void AddThisPanel() => panels.Add(this);

        public static void AddPanel(PanelBehaviour panel) => panels.Add(panel);

        public virtual void SetOpenPanel(bool open)
        {
            opened = open;

            if (open)
            {
                previousPanel = currentPanel;
                currentPanel = this;
            }
        }

        public static void ReturnToMain(bool closeAll = false)
        {
            var copyPanels = panels.ToArray();
            foreach (var panel in copyPanels)
                if (panel == null)
                    panels.Remove(panel);

            panels.ForEach((panel) => { if (panel.opened || closeAll) panel.SetOpenPanel(panel.main); });
        }

        public static void ReturnToPrevious()
        {
            currentPanel.SetOpenPanel(false);

            if (previousPanel.main)
                ReturnToMain();
            else
                previousPanel.SetOpenPanel(true);
        }
    }
}
