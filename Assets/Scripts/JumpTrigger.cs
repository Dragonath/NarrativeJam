using UnityEngine;

public class JumpTrigger : MonoBehaviour
{
    public GameObject VFX;
    private void OnDestroy()
    {
        if (SaveAndLoad.instance != null)
        {
            Instantiate(VFX, transform.position, Quaternion.identity);
            SoundManager.PlaySound("pickupSFX");
            SaveAndLoad.instance._jumpUnlocked = true;
            SaveAndLoad.instance.Save();
        }
    }
}
