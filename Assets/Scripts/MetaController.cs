using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

public class MetaController : MonoBehaviour
{
    public GameObject camerasParent;
    public GameObject fpsFrontPanel;

    CharacterController controller;
    
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Assert.IsTrue(controller != null);
        Assert.IsTrue(camerasParent != null);
        Assert.IsTrue(fpsFrontPanel != null);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFPS();
        camerasParent.transform.position = gameObject.transform.position;
    }

    void UpdateFPS()
	{
        // Move controller
        float strafe = Input.GetAxis("Horizontal");
        float forwards = Input.GetAxis("Vertical");
        controller.Move(((controller.transform.forward * forwards) + (controller.transform.right * strafe)) * Time.deltaTime);

        float horizontal = Input.GetAxis("Mouse X");
        float vertical = Input.GetAxis("Mouse Y");
        // Rotate controller
        {
            Vector3 controllerCurrent = controller.transform.rotation.eulerAngles;
            float controllerNewY = controllerCurrent.y + horizontal;
            Vector3 controllerNewRot = Quaternion.Euler(new Vector3(0.0f, controllerNewY, 0.0f)).eulerAngles;
            if (controllerNewRot.y > 70.0f && controllerNewRot.y < 180.0f)
                controllerNewRot.y = 70.0f;
            if (controllerNewRot.y >= 180.0f && controllerNewRot.y < 290.0f)
                controllerNewRot.y = 290.0f;
            controller.transform.rotation = Quaternion.Euler(controllerNewRot);
        }

        // Rotate camera
        {
            Vector3 current = fpsFrontPanel.transform.rotation.eulerAngles;
            float newX = current.x - vertical;
            float newY = current.y + horizontal;
            Vector3 newRot = Quaternion.Euler(new Vector3(newX, newY, 0.0f)).eulerAngles;
            if (newRot.x > 70.0f && newRot.x < 180.0f)
                newRot.x = 70.0f;
            if (newRot.x >= 180.0f && newRot.x < 290.0f)
                newRot.x = 290.0f;
            if (newRot.y > 70.0f && newRot.y < 180.0f)
                newRot.y = 70.0f;
            if (newRot.y >= 180.0f && newRot.y < 290.0f)
                newRot.y = 290.0f;
            fpsFrontPanel.transform.rotation = Quaternion.Euler(newRot);
        }
    }
}
