using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DavidJalbert.TinyCarControllerAdvance
{
    public class ExampleGui : MonoBehaviour
    {
        public static ExampleGui instance;
        public static List<string> additionalInfo = new List<string>();

        [System.Serializable]
        public class CarType
        {
            public TCCAPlayer carObject;
            public string description;
        }

        public Text textDebug;
        public Text textDescription;
        public CarType[] carTypes;
        public TCCAPlayer carController;
        public TCCACamera carCamera;
        public TCCAMobileInput mobileInput;
        public TCCAStandardInput standardInput;

        private int carIndex = 0;

        private void Start()
        {
            instance = this;
            changeCar(0);
        }

        void LateUpdate()
        {
            if (textDebug == null) return;

            textDebug.text = "";

            if (carController != null)
            {
                textDebug.text += "Car : " + carTypes[carIndex].description + "\n";
                textDebug.text += "Speed : " + (int)carController.getForwardVelocity() + " m/s\n";
                textDebug.text += "Drift speed : " + (int)carController.getLateralVelocity() + " m/s\n";
                textDebug.text += "Is grounded : " + (carController.isFullyGrounded() ? "true" : (carController.isGrounded() ? "partial" : "false")) + "\n";
                textDebug.text += "Pitch : " + (int)(carController.getCarBody().getPitchAngle()) + " degrees\n";
                textDebug.text += "Roll : " + (int)(carController.getCarBody().getRollAngle()) + " degrees\n";

                foreach (string line in additionalInfo)
                {
                    textDebug.text += line + "\n";
                }

                additionalInfo.Clear();
            }
        }

        public static void addInfo(string line)
        {
            additionalInfo.Add(line);
        }

        public static bool isCarSelected(TCCAPlayer p)
        {
            return instance.carController == p;
        }

        private void changeCar(int i)
        {
            if (carController != null)
            {
                carController.setBoost(0);
                carController.setHandbrake(false);
                carController.setMotor(0);
                carController.setSteering(0);
            }

            carIndex = i % carTypes.Length;
            carController = carTypes[carIndex].carObject;

            carCamera.carController = carController;
            mobileInput.carController = carController;
            standardInput.carController = carController;

            carCamera?.resetCamera();
        }

        public void onClickChangeCar()
        {
            changeCar(carIndex + 1);
        }

        public void onClickMobileInput()
        {
            mobileInput.gameObject.SetActive(!mobileInput.gameObject.activeSelf);
        }

        public void onClickCameraAngle()
        {
            if (carCamera != null)
            {
                switch (carCamera.viewMode)
                {
                    case TCCACamera.CAMERA_MODE.TopDown:
                        carCamera.viewMode = TCCACamera.CAMERA_MODE.ThirdPerson;
                        break;
                    case TCCACamera.CAMERA_MODE.ThirdPerson:
                        carCamera.viewMode = TCCACamera.CAMERA_MODE.TopDown;
                        break;
                }
                carCamera.resetCamera();
            }
        }

        public void onClickDescriptionText()
        {
            textDebug.enabled = !textDebug.enabled;
            textDescription.enabled = !textDescription.enabled;
        }
    }
}