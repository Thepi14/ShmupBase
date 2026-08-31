using Main.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Main.UI
{
    public class ControlRebindSelectionNotifier : MonoBehaviour, ISelectHandler
    {
        public void OnSelect(BaseEventData eventData)
        {
            SettingsPanel.CurrentSelectedControlRebindSelectable = gameObject.GetComponent<Selectable>();
        }
    }
}
