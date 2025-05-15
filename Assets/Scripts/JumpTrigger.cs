using UnityEngine;

public class JumpTrigger : MonoBehaviour
{
    private void OnDestroy()
    {
        if (SaveAndLoad.instance != null)
        {
            // TO ADD: VFX & SFX
            SaveAndLoad.instance._jumpUnlocked = true;
            SaveAndLoad.instance.Save();
        }
    }
}
