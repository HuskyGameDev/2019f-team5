//
// When We Fell
//

using UnityEngine;

public sealed class LightProcessor : MonoBehaviour
{
	public Material mat;
	public Camera scanner;

	private void Start()
		=> CreateLightmap();

	private void CreateLightmap()
	{
		RenderTexture lightmap = new RenderTexture(Screen.width, Screen.height, 0);
		scanner.targetTexture = lightmap;

		mat.SetTexture("_Lightmap", lightmap);
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
		=> Graphics.Blit(source, destination, mat);
}