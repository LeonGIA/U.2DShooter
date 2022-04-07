using UnityEngine;

/**

TODO
    1. Create and add enemy attack animation
    3. Add some sort of pathing that prevents enemies from running off a cliff.
    4. Find a better way to handle attacking the player.
        NOTE: Right now, the dinosaur enemy uses a separate circle collider located near the sprite's mouth.
        This is to prevent the player from taking damage when attacking (landing on top of the dinosaur).
        flip() was created to maintain the collider position

*/

public class EnemyController : MonoBehaviour
{
    public int enemyHealth = 100;
    private float CHASE_DISTANCE = 15f;
    private Vector2 playerDistance;

    private GameObject player;
    private Transform playerLocation;
    private Rigidbody2D rb2d;
    // private enum enemyState {idle, run, attack}
    // private enemyState state;
    // private bool facingRight = true;

    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        rb2d = GetComponent<Rigidbody2D>();
        playerLocation = player.GetComponent<Transform>();
    }

    void Update()
    {
        // Calculates distance to player
        playerDistance = new Vector2(playerLocation.position.x - transform.position.x, 0f);

        Debug.Log(playerDistance);
        
        // if(rb2d.velocity.x > .1f && !facingRight)   
        //     flip();
        // else if(rb2d.velocity.x < -.1f && facingRight)
        //     flip();

        // updateEnemyState();
    }

    void FixedUpdate()
    {
        // Moves enemy only if the distance to player is less than the pre-set chase distance.
        if((Vector2.Distance(transform.position, playerLocation.position) < CHASE_DISTANCE) && playerDistance.normalized != Vector2.zero)
        {
            rb2d.AddForce(playerDistance.normalized, ForceMode2D.Impulse);
        }
    }

    // Flips enemy using localscale instead of sprite.flipx so that the circle collider maintains its position
    // private void flip()
    // {
    //     Vector3 currentScale = transform.localScale;
    //     currentScale.x *= -1;

    //     transform.localScale = currentScale;
    //     facingRight = !facingRight;
    // }

    // private void updateEnemyState()
    // {
    //     if(rb2d.velocity.x > .1f || rb2d.velocity.x < -.1f)
    //     {
    //         state = enemyState.run;
    //     }
    //     else if(Mathf.Approximately(rb2d.velocity.x, 0f) && Mathf.Approximately(rb2d.velocity.y, 0f))
    //     {
    //         state = enemyState.idle;
    //     }
    // }

    public void takeDamage(int damage)
    {
        enemyHealth -= damage;
        if(enemyHealth <= 0)
            Destroy(this.gameObject);
    }

    // Runs when a collision is detected and continues until the collision stops.
    // void OnTriggerStay2D(Collider2D col)
    // {
    //     GameObject objectHit = col?.gameObject;
    //     if(objectHit.tag == "Player")
    //     {
    //         PlayerHealthController playerReference = objectHit.GetComponent<PlayerHealthController>();
    //         if(playerReference.getHealth() > 0)
    //             playerReference.damagePlayer(100);
    //         else  
    //             return;
    //     }
    // }
}
