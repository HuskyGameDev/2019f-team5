//
// When We Fell
//

using UnityEngine;
using UnityEngine.U2D;

public class GameAssets : MonoBehaviour
{
	private static GameAssets instance;

	public SpriteAtlas sprites;
	public GameObject spritePrefab;

	public static GameAssets Instance
	{
		get
		{
			if (instance == null)
				instance = GameObject.FindWithTag("GameAssets").GetComponent<GameAssets>();

			return instance;
		}
	}
}
