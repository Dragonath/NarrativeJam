using UnityEngine;

public class DashTrigger : MonoBehaviour
{
    private void OnDestroy()
    {
        if (SaveAndLoad.instance != null)
        {
            SoundManager.PlaySound("pickupSFX");
            SaveAndLoad.instance._dashUnlocked = true;
            SaveAndLoad.instance._maxJumpCount = 1;
            SaveAndLoad.instance._lvl2Flag = 1;
            SaveAndLoad.instance.Save();
        }
    }
}
