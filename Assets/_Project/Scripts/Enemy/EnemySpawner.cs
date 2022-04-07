using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	[SerializeField] private GameObject enemyPrefab;
	
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.E))
		{
			spawnEnemy();
		}
	}

	private void spawnEnemy()
	{
		Instantiate(enemyPrefab, transform.position, Quaternion.identity);
	}
}
