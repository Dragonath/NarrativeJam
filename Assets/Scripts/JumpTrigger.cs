using UnityEngine;
using UnityEngine.Audio;

public class JumpTrigger : MonoBehaviour
{
    private void OnDestroy()
    {
        if (SaveAndLoad.instance != null && !GameManager.instance.inDialogue && GameManager.instance.noEarlyUnlocks && SaveAndLoad.instance._lvl1Flag < 1)
        {
            SoundManager.PlaySound("pickupSFX");
            SaveAndLoad.instance._jumpUnlocked = true;
            Player_Controller.instance.jumpUnlocked = true;
            SaveAndLoad.instance._maxJumpCount = 1;
            SaveAndLoad.instance._lvl1Flag = 1;
        }
    }
}
