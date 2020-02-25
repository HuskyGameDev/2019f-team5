//
// When We Fell
//

using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 1.0f);

        MeshRenderer rend = GetComponent<MeshRenderer>();
        rend.sortingLayerName = "Default";
        rend.sortingOrder = 2;
    }

    private void Update()
    {
        transform.localPosition += new Vector3(0, 1.0f * Time.deltaTime, 0);
    }
}
