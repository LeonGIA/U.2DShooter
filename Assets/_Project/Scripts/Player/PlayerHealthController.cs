using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealthController : MonoBehaviour
{
    private int playerHealth = 100;
    // private Animator playerAnimator;
    private Rigidbody2D rb2d;
    // private GameObject gameController;

    // Start is called before the first frame update
    void Start()
    {
        // playerAnimator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        // gameController = GameObject.FindWithTag("GameController");
    }

    public int getHealth()
    {
        return playerHealth;
    }

    public void damagePlayer(int damage)
    {
        playerHealth -= damage;
        if(playerHealth <= 0)
        {
            death();
        }
    }

    private void death()
    {
        // playerAnimator.SetTrigger("death");
        rb2d.bodyType = RigidbodyType2D.Static;
        SceneManager.LoadScene(SceneManager.GetActiveScene().ToString());
    }

    // private void restartLevel()
    // {
    //     gameController.GetComponent<GameController>().endGame();
    // }
}
