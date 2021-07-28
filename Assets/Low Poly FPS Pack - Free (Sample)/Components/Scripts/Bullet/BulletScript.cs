using UnityEngine;
using System.Collections;

// ----- Low Poly FPS Pack Free Version -----
public class BulletScript : MonoBehaviour {

	[Range(5, 100)]
	[Tooltip("After how long time should the bullet prefab be destroyed?")]
	public float destroyAfter = 5f;

	[Header("Impact Effect Prefabs")]
	public Transform [] metalImpactPrefabs;

    private void Start () 
	{
		//Start destroy timer
		StartCoroutine(DestroyAfter());
	}
	RaycastHit hit;
	public float maxDistance = 1;
	public LayerMask layerMask;

	private void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance, layerMask))
        {
			hit.transform.GetComponent<ZombieAction>().TakeHit(bulletPower);
			Debug.Log("레이케스트 좀비 맞음");
		}
        //var colliders = Physics.OverlapSphere(transform.position, gizmosSize);
        //foreach (var item in colliders)
        //{
        //    if (item.transform.CompareTag("Zombie") == true)
        //    {
        //        item.GetComponent<ZombieAction>().TakeHit(bulletPower);

        //        Debug.Log("오버렙스피어 좀비 맞음");
        //        Destroy(this.gameObject);
        //    }
        //}
    }
    public float gizmosSize = 1;
    private void OnDrawGizmos()
    {
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, gizmosSize);
    }
    public float bulletPower = 5f;
	//private void OnTriggerEnter(Collider other)
	//{
	//	if (other.transform.CompareTag("Zombie") == true)
	//	{
	//		ZombieAction.instance.TakeHit(bulletPower);
	//		Debug.Log("온트리거 좀비 맞음");
	//	}
	//}

	private IEnumerator DestroyAfter () 
	{
		//Wait for set amount of time
		yield return new WaitForSeconds (destroyAfter);
		//Destroy bullet object
		Destroy (gameObject);
	}
}
// ----- Low Poly FPS Pack Free Version -----