using UnityEngine;

public class PlayerEyeDamage : MonoBehaviour{
	[SerializeField] private float eyeDamage = 5;
	private void OnTriggerEnter(Collider col){
		Debug.Log(col.gameObject.name);
		if (col.gameObject.TryGetComponent<EyeEnemy>(out var eye)){
			GetComponent<ITakeHit>().TakeHit(5, col.gameObject.transform.position, "EYE");
		}
	}
}
