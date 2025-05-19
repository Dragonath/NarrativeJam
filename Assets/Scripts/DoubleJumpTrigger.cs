using UnityEngine;

public class DoubleJumpTrigger : MonoBehaviour
{


    private void OnDestroy()
    {
        if (SaveAndLoad.instance != null && !GameManager.instance.inDialogue && GameManager.instance.noEarlyUnlocks && SaveAndLoad.instance._lvl3Flag < 1)
        {
            Player_Controller.instance.ToggleAnimator();
            SoundManager.PlaySound("pickupSFX");
            SaveAndLoad.instance._maxJumpCount = 2;
            SaveAndLoad.instance._lvl3Flag = 1;
        }
    }
}
