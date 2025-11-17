using UnityEngine;

public class CursorLock : MonoBehaviour
{
    void OnGUI()
    {
        if(Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
        {
            SwitchCursorLock(true);
        }
        else if(Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            SwitchCursorLock(false);
        }
    }

    void SwitchCursorLock(bool mustBeShown)
    {
        Cursor.visible = mustBeShown;
        Cursor.lockState = mustBeShown ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
