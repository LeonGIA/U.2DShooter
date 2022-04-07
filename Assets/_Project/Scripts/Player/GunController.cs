using UnityEngine;
using System.Collections;

public class GunController : MonoBehaviour
{

	[SerializeField] private GameObject bulletEmitter;
	[SerializeField] private GameObject muzzleFlash;
	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private GameObject playerArm; // used to keep track of rotation of the arm

	private AudioSource shootSound;

	private float MUZZLE_FLASH_DURATION = 10f;
	private float FIRE_RATE = .1f;
	private float BULLET_FORCE = 50f;
	private bool firingGun = false;

	void Awake()
	{
		shootSound = GetComponent<AudioSource>();
	}

	void Start()
	{
		muzzleFlash.SetActive(false);
	}

    void Update()
	{
		if(Input.GetMouseButton(0))
		{
			if(!firingGun)
			{
				StartCoroutine(fireGun());
			}
		}
	}

	IEnumerator fireGun()
	{
		muzzleFlash.SetActive(true);
		firingGun = true;

		shootSound.Play();

		// Instantiates bullet and adds force
		GameObject temporaryBullet = Instantiate(bulletPrefab, bulletEmitter.transform.position, playerArm.transform.rotation) as GameObject;
		temporaryBullet.GetComponent<Rigidbody2D>().AddForce(temporaryBullet.transform.right * BULLET_FORCE, ForceMode2D.Impulse);

		sendRay(); // Raycast for hit detection, bullets are just sprites
		
		/*
			Could use colliders on bullets w/ OnTriggerEnter. Should be used if bullet spread is something to be taken into consideration.
		*/

		// Leaves muzzle flash visible for specified duration in frames. 
		float durationShown = 0f;
		while(durationShown < MUZZLE_FLASH_DURATION)
		{
			durationShown++;
			yield return null;
		}
		muzzleFlash.SetActive(false);

		yield return new WaitForSecondsRealtime(FIRE_RATE); // Limits fire rate of the gun
		Destroy(temporaryBullet); // Destroys bullet according to fire rate
		firingGun = false;
	}

	private void sendRay()
	{
		RaycastHit2D rayCast = Physics2D.Raycast(bulletEmitter.transform.position, bulletEmitter.transform.right, 10f);
		Debug.DrawRay(bulletEmitter.transform.position, bulletEmitter.transform.right * 10f, Color.red, 2f);
		
		Collider2D objectHit = rayCast.collider;

		if(objectHit?.tag == "Enemy")
		{
			objectHit.GetComponent<EnemyController>().takeDamage(100);
		}
	}
}
