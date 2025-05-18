using UnityEngine;

public class DashTrigger : MonoBehaviour
{
    private void OnDestroy()
    {
        if (SaveAndLoad.instance != null && !GameManager.instance.inDialogue && GameManager.instance.noEarlyUnlocks)
        {
            SoundManager.PlaySound("pickupSFX");
            SaveAndLoad.instance._dashUnlocked = true;
            Player_Controller.instance.dashUnlocked = true;
            SaveAndLoad.instance._maxJumpCount = 1;
            SaveAndLoad.instance._lvl2Flag = 1;
        }
    }
}
