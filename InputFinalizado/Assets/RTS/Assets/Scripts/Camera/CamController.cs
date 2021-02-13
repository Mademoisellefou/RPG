using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class CamController : MonoBehaviour {
	
	//variables not visible in the inspector
	public static float movespeed;
	public static float zoomSpeed;
	public static float mouseSensitivity;
    public static float clampAngle;
	
    float rotationY = 0;
    float rotationX = 0;
	
	bool canRotate;
 
    void Start(){
		//get start rotation
		Vector3 rot = transform.eulerAngles;
		rotationY = rot.y;
		rotationX = rot.x;
    }
	
	void Update(){
			PcCamera();
	
		
	}
	
	void PcCamera(){
		//if key gets pressed move left/right
		if(Input.GetKey("a")){
			transform.Translate(Vector3.right * Time.deltaTime * -movespeed);
		}
		if(Input.GetKey("d")){
			transform.Translate(Vector3.right * Time.deltaTime * movespeed);
		}
	
		//if key gets pressed move up/down
		if(Input.GetKey("w")){
			transform.Translate(Vector3.up * Time.deltaTime * movespeed);
		}
		if(Input.GetKey("s")){
			transform.Translate(Vector3.up * Time.deltaTime * -movespeed);
		}
	
		//if scrollwheel is down rotate camera
		if(Input.GetMouseButton(2)){
			float mouseX = Input.GetAxis("Mouse X");
			float mouseY = -Input.GetAxis("Mouse Y");
			RotateCamera(mouseX, mouseY);
		}
	
		//move camera when you scroll
		transform.Translate(new Vector3(0, 0, Input.GetAxis("Mouse ScrollWheel")) * Time.deltaTime * zoomSpeed);
	}
	
	

	
	void RotateCamera(float mouseX, float mouseY){
		//check if mobile controls are enabled to adjust sensitivity
		if(GameObject.Find("Mobile") == null && GameObject.Find("Mobile multiplayer") == null){
			rotationY += mouseX * mouseSensitivity * Time.deltaTime;
			rotationX += mouseY * mouseSensitivity * Time.deltaTime;
		}
		else{
			rotationY += mouseX * mouseSensitivity * Time.deltaTime * 0.02f;
			rotationX += mouseY * mouseSensitivity * Time.deltaTime * 0.02f;	
		}
	
		//clamp x rotation to limit it
		rotationX = Mathf.Clamp(rotationX, -clampAngle, clampAngle);
	
		//apply rotation
		transform.rotation = Quaternion.Euler(rotationX, rotationY, 0.0f);
	}
}
