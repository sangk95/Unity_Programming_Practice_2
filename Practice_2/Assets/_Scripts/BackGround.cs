using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    [SerializeField]
    float speed;
    int startIndex=2;
    int endIndex=0;
    float viewHeight;
    [SerializeField]
    Transform[] sprites;
    void Awake()
    {
        viewHeight = Camera.main.orthographicSize * 2;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 curPosition = transform.position;
        Vector3 nextPosition = Vector3.down * speed * Time.deltaTime;
        transform.position = curPosition + nextPosition;

        if(sprites[endIndex].position.y < viewHeight*(-1))
        {
            Vector3 backSpritePosition = sprites[startIndex].localPosition;
            Vector3 frontSpritePosition = sprites[endIndex].localPosition;
            sprites[endIndex].transform.localPosition = backSpritePosition + Vector3.up * viewHeight;

            int temp = startIndex;
            startIndex = endIndex;
            endIndex = (temp-1 == -1)? sprites.Length-1 : temp-1;
        }
    }
}
