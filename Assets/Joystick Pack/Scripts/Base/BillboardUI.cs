using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        // «мушуЇ UI над бочкою завжди бути розвернутим до гравц€
        transform.LookAt(transform.position + cam.forward);
    }
}