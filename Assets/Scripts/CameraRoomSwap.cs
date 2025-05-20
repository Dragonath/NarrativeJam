using UnityEngine;
public class CameraRoomSwap : MonoBehaviour
{
    public Grid grid;
    public GameObject player;
    public GameObject mainCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3Int cellPos = grid.WorldToCell(player.transform.position);
        mainCamera.transform.position = grid.GetCellCenterWorld(cellPos);
    }
}
