//
// When We Fell
//

using UnityEngine;
using UnityEngine.U2D;

// A basic way to handle assets. We store them as references in here.
// The rest of the game can access the singleton to get them.
// This requires modifying the scene file to add assets, though.
public class GameAssets : MonoBehaviour
{
	private static GameAssets instance;

	public SpriteAtlas sprites;
	public GameObject tileRectPrefab;

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
