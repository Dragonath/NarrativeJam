using UnityEngine;

public class IntroLogic : MonoBehaviour
{
    private Animator animator;

    public static IntroLogic Instance { get; private set; }

    private void Awake()
    {
        Instance = this; 
    }
    private void Start()
    {
        animator = GetComponent<Animator>();

    }

    public void StartIntroAnimation()
    {
        animator.Play("Unburrow");
    }

    public void WalkOutOfIntro()
    {
        animator.Play("LeaveIntro");
    }

    public void LoadNextScene()
    {
        GameManager.instance.LoadScene(2);
    }
}
