using UnityEngine;

public class JumpTrigger : MonoBehaviour
{
    private void OnDestroy()
    {
        if (SaveAndLoad.instance != null)
        {
            SoundManager.PlaySound("pickupSFX");
            SaveAndLoad.instance._jumpUnlocked = true;
            SaveAndLoad.instance.Save();
        }
    }
}
