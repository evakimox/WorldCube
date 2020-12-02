using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRaycastHitTest : Singleton<CubeRaycastHitTest>
{
    public bool isHit(Camera cubeCamera, Camera worldCamera, Vector2 screenPosition, Collider obj, string prefex = "")
    {
        Ray cubeCamRay = cubeCamera.ScreenPointToRay(screenPosition);
        RaycastHit[] cubeHits;
        cubeHits = Physics.RaycastAll(cubeCamRay, 100f);

        Vector2? RTCoordinate = null;
        foreach(RaycastHit hit in cubeHits)
        {
            if (hit.collider.gameObject.name.Contains(prefex))
            {
                //Debug.Log(hit.collider.gameObject.name + ": " + hit.textureCoord);
                RTCoordinate = hit.textureCoord;
                break;
            }
        }
        if (RTCoordinate != null) 
        {
            Vector2 coord = (Vector2)RTCoordinate;
            Vector2 worldCamScreenPoint = new Vector2(coord.x * worldCamera.pixelWidth, coord.y * worldCamera.pixelHeight);

            Ray worldRay = worldCamera.ScreenPointToRay(worldCamScreenPoint);
            RaycastHit worldHit;
            Physics.Raycast(worldRay, out worldHit);
            if(worldHit.collider == obj)
            {
                return true;
            }
        }
        return false;
    }
}
