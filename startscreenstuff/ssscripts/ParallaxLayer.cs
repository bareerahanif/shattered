using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    public float scrollSpeed = 0.5f;
    private float spriteWidth;

    void Start()
    {
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        transform.position += Vector3.left * scrollSpeed * Time.deltaTime;

        if (transform.position.x <= -spriteWidth)
            transform.position += new Vector3(spriteWidth * 2, 0, 0);
    }
}
