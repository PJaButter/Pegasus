using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform transformToFollow;
    public float minBorderX;
    public float maxBorderX;
    public float minBorderY;
    public float maxBorderY;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.RefreshPosition();
    }

    public void SetBorders(float minX, float maxX, float minY, float maxY)
    {
        minBorderX = minX;
        maxBorderX = maxX;
        minBorderY = minY;
        maxBorderY = maxY;

        this.RefreshPosition();
    }

    private void RefreshPosition()
    {
        if (transformToFollow != null)
        {
            float newX = transformToFollow.position.x;
            if (newX > maxBorderX)
                newX = maxBorderX;
            else if (newX < minBorderX)
                newX = minBorderX;
            float newY = transformToFollow.position.y;
            if (newY > maxBorderY)
                newY = maxBorderY;
            else if (newY < minBorderY)
                newY = minBorderY;
            this.transform.position = new Vector3(newX, newY, this.transform.position.z);
        }
    }
}
