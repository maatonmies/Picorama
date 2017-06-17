using UnityEngine;
using System.Collections;

public class GradientBackground : MonoBehaviour
{	
	public Color topColor = Color.blue;
	public Color bottomColor = Color.white;
	public Color flashColor = Color.red;
	
	public Color chapter1Top = Color.red;
	public Color chapter1Bottom = Color.blue;
	public Color chapter2Top = Color.red;
	public Color chapter2Bottom = Color.blue;
	public Color chapter3Top = Color.red;
	public Color chapter3Bottom = Color.blue;
	public Color chapter4Top = Color.red;
	public Color chapter4Bottom = Color.blue;
	public Color chapter5Top = Color.red;
	public Color chapter5Bottom = Color.blue;
	public Color chapter6Top = Color.red;
	public Color chapter6Bottom = Color.blue;
	public Color chapter7Top = Color.red;
	public Color chapter7Bottom = Color.blue;

	public int gradientLayer = 7;

	public GameObject bgLightWhirl;
	private GameObject gradientPlane;
	private Mesh mesh;

	private Camera mainCamera;

	void Awake ()
	{	
		gradientLayer = Mathf.Clamp (gradientLayer, 0, 31);
		if (!GetComponent<Camera> ()) {
			Debug.LogError ("Must attach GradientBackground script to the camera");
			return;
		}
		
		GetComponent<Camera> ().clearFlags = CameraClearFlags.Depth;
		GetComponent<Camera> ().cullingMask = GetComponent<Camera> ().cullingMask & ~(1 << gradientLayer);
		Camera gradientCam = new GameObject ("Gradient Cam", typeof(Camera)).GetComponent<Camera> ();
		gradientCam.transform.position = new Vector3 (-10000, 0, 0);
		gradientCam.depth = GetComponent<Camera> ().depth - 1;
		gradientCam.cullingMask = 1 << gradientLayer;
		
		mesh = new Mesh ();
		mesh.vertices = new Vector3[4] {
						new Vector3 (-100f, .577f, 1f),
						new Vector3 (100f, .577f, 1f),
						new Vector3 (-100f, -.577f, 1f),
						new Vector3 (100f, -.577f, 1f)
				};
		
		mesh.colors = new Color[4] {topColor,topColor,bottomColor,bottomColor};
		
		mesh.triangles = new int[6] {0, 1, 2, 1, 3, 2};
		
		Material mat = new Material ("Shader \"Vertex Color Only\"{Subshader{BindChannels{Bind \"vertex\", vertex Bind \"color\", color}Pass{}}}");
		gradientPlane = new GameObject ("Gradient Plane", typeof(MeshFilter), typeof(MeshRenderer));
		gradientPlane.transform.position = new Vector3 (-10000, 0, 0);
		
		
		((MeshFilter)gradientPlane.GetComponent (typeof(MeshFilter))).mesh = mesh;
		gradientPlane.GetComponent<Renderer> ().material = mat;
		gradientPlane.layer = gradientLayer;

		mainCamera = GameObject.Find ("Camera").GetComponent<Camera> ();
		Vector3 screenCenter = mainCamera.ScreenToWorldPoint (new Vector3 (Screen.width / 2, Screen.height / 2));
		Vector3 lightPos = new Vector3 (screenCenter.x, screenCenter.y, 0);

		transform.position = new Vector3 (screenCenter.x, screenCenter.y, -250);

		Instantiate (bgLightWhirl, lightPos, Quaternion.identity);
	}

	private void ChangeToLightBg ()
	{
		mesh.colors = new Color[4] {flashColor,flashColor,flashColor,flashColor};
		((MeshFilter)gradientPlane.GetComponent (typeof(MeshFilter))).mesh = mesh;
	}

	private void FlashToLightBg ()
	{
		mesh.colors = new Color[4] {flashColor,flashColor,flashColor,flashColor};
		((MeshFilter)gradientPlane.GetComponent (typeof(MeshFilter))).mesh = mesh;

		Invoke ("BackToNormalColors", 0.4f);
	}

		
	private void BackToNormalColors ()
	{
		mesh.colors = new Color[4] {topColor,topColor,bottomColor,bottomColor};
		((MeshFilter)gradientPlane.GetComponent (typeof(MeshFilter))).mesh = mesh;
	}
	
	private void ChangeChapterBG (int currentScreen)
	{
		switch (currentScreen) {
		case 0:
			mesh.colors = new Color[4] {chapter1Top,chapter1Top,chapter1Bottom,chapter1Bottom};
			((MeshFilter)gradientPlane.GetComponent (typeof(MeshFilter))).mesh = mesh;
			break;
		case 1:
			mesh.colors = new Color[4] {chapter2Top,chapter2Top,chapter2Bottom,chapter2Bottom};
			((MeshFilter)gradientPlane.GetComponent (typeof(MeshFilter))).mesh = mesh;
			break;
		case 2:
			mesh.colors = new Color[4] {chapter3Top,chapter3Top,chapter3Bottom,chapter3Bottom};
			((MeshFilter)gradientPlane.GetComponent (typeof(MeshFilter))).mesh = mesh;
			break;
		case 3:
			mesh.colors = new Color[4] {chapter4Top,chapter4Top,chapter4Bottom,chapter4Bottom};
			((MeshFilter)gradientPlane.GetComponent (typeof(MeshFilter))).mesh = mesh;
			break;
		case 4:
			mesh.colors = new Color[4] {chapter5Top,chapter5Top,chapter5Bottom,chapter5Bottom};
			((MeshFilter)gradientPlane.GetComponent (typeof(MeshFilter))).mesh = mesh;
			break;
		case 5:
			mesh.colors = new Color[4] {chapter6Top,chapter6Top,chapter6Bottom,chapter6Bottom};
			((MeshFilter)gradientPlane.GetComponent (typeof(MeshFilter))).mesh = mesh;
			break;
		case 6:
			mesh.colors = new Color[4] {chapter7Top,chapter7Top,chapter7Bottom,chapter7Bottom};
			((MeshFilter)gradientPlane.GetComponent (typeof(MeshFilter))).mesh = mesh;
			break;
		}
	}
	
}
