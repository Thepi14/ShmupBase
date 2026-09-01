using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Main.UI
{
    public class CategoryButtonNavigationUpdater : MonoBehaviour, ISelectHandler
    {
        public void OnSelect(BaseEventData eventData)
        {
            if (!Vars.UseMouse)
                SettingsPanel.Instance.UpdateCategoryNavigation(GetComponent<Button>());
        }
    }
}
