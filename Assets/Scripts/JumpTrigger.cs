using UnityEngine;

public class JumpTrigger : MonoBehaviour
{
    private void OnDestroy()
    {
        if (SaveAndLoad.instance != null)
        {
            SoundManager.PlaySound("pickupSFX");
            SaveAndLoad.instance._jumpUnlocked = true;
            SaveAndLoad.instance._maxJumpCount = 1;
            SaveAndLoad.instance._lvl1Flag = 1;
            SaveAndLoad.instance.Save();
        }
    }
}
