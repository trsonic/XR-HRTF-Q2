using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphericalGrid : MonoBehaviour
{
    void Start()
    {
        float horizonDistance = 0.6f;
        createHorizonMesh(horizonDistance, .01f * horizonDistance);
    }

    void Update()
    {
        
    }

    void createHorizonMesh(float distance, float lineWidth)
    {
        //horizonContainer.transform.position = new Vector3();
        //horizonContainer.transform.rotation = new Quaternion();

        for (int i = 0; i < 18 * 2; i++)
        {
            Vector3 targetPosition;

            float alpha = (float)i * 5.0f;
            float vertPosition = Mathf.Cos(alpha * (Mathf.PI / 180)) * distance;
            float radius = Mathf.Sin(alpha * (Mathf.PI / 180)) * distance;

            targetPosition.x = 0.0f;
            targetPosition.y = vertPosition;
            targetPosition.z = 0.0f;

            var circle = new GameObject { name = "Circle" };
            circle.DrawCircle(radius, lineWidth);

            GameObject objectClone = Instantiate(circle, targetPosition, new Quaternion());
            objectClone.transform.parent = transform;

            if (i == 18) objectClone.GetComponent<LineRenderer>().material.color = Color.red;
            else objectClone.GetComponent<LineRenderer>().material.color = Color.white;
            objectClone.GetComponent<LineRenderer>().generateLightingData = true;
            objectClone.name = "horizontalCircle" + i;

            Destroy(circle);
        }

        for (int i = 0; i < 18 * 2; i++)
        {
            float radius = distance;
            float rotation = (float)i * 5.0f;

            var circle = new GameObject { name = "Circle" };
            circle.DrawCircle(radius, lineWidth);

            GameObject objectClone = Instantiate(circle, new Vector3(), new Quaternion());
            objectClone.transform.parent = transform;
            objectClone.transform.Rotate(0.0f, rotation, 90.0f);
            if (i == 0) objectClone.GetComponent<LineRenderer>().material.color = Color.red;
            else objectClone.GetComponent<LineRenderer>().material.color = Color.white;
            objectClone.GetComponent<LineRenderer>().generateLightingData = true;
            objectClone.name = "verticalCircle" + i;

            Destroy(circle);
        }
    }

    void deleteHorizon()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
