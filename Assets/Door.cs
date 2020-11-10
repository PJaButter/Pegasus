using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject gameObjectDoorLeadsTo;
    public Area area;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UsedDoor(GameObject player)
    {
        if (GameManager.Get.CurrentAreaID != area.ID)
        {
            player.transform.SetParent(area.gameObject.transform);
            GameManager.Get.CurrentAreaID = area.ID;
            CameraFollow cameraFollow = GameManager.Get.WorldCamera.GetComponent<CameraFollow>();
            if (cameraFollow != null)
            {
                cameraFollow.SetBorders(area.minBorderX, area.maxBorderX, area.minBorderY, area.maxBorderY);
            }
        }

        player.transform.position = new Vector3(gameObjectDoorLeadsTo.transform.position.x, gameObjectDoorLeadsTo.transform.position.y, player.transform.position.z);
    }
}
