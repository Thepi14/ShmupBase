using UnityEngine;

namespace Main.UI
{
    public class GenericPanelBehaviour : PanelBehaviour
    {
        public RectTransform subPanel;

        public override void SetOpenPanel(bool open, bool overridePrevious = false)
        {
            if (open)
                foreach (PanelBehaviour panel in panels)
                    if (panel != this && !panel.main)
                        panel.SetOpenPanel(false);

            base.SetOpenPanel(open);

            if (background != null)
                background.enabled = open;

            subPanel.gameObject.SetActive(open);
        }
    }
}
