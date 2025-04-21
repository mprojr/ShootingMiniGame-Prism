using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public class WallDotMovement : MonoBehaviour
{
    public GameObject playerCamera;
    public float speed = 2.0f;
    private bool isMoving = false;
    private bool shrinking = false;
    private Renderer dotRenderer;

    void Start()
    {
        dotRenderer = GetComponent<Renderer>();
        if (playerCamera == null)
        {
            playerCamera = GameObject.FindGameObjectWithTag("Player");
            //Debug.Log("FOUND THE PLAYER FROM WALLDOTMOV");
        }
        StartCoroutine(GrowStage());
    }

    void Update()
    {
        if (isMoving && !shrinking)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerCamera.transform.position, speed * Time.deltaTime);
        }       
    }

    IEnumerator GrowStage()
    {
        float growTime = 3.0f;
        float elapsedTime = 0f;
        Vector3 initialScale = transform.localScale;
        Vector3 tragetScale = initialScale * 2.0f;
        // Debug.Log("From GrowStage");

        while (elapsedTime < growTime)
        {
            transform.localScale = Vector3.Lerp(initialScale, tragetScale, elapsedTime / growTime);
            elapsedTime += Time.deltaTime;
            yield return null;
            // Debug.Log("From GrowStage");
        }

        transform.localScale = tragetScale;
        dotRenderer.material.color = Color.yellow;
        isMoving = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shrinking = true;
            StartCoroutine(ShrinkAndDestroy(Color.red));
            GameManager.Instance.TakeDamage(1);
        } else if (other.CompareTag("Bullet"))
        {
            StartCoroutine(ShrinkAndDestroy(Color.blue));
            //Destroy(other.gameObject);
        }
    }

    IEnumerator ShrinkAndDestroy(Color shrinkColor)
    {
        float shrinkTime = 2.0f;
        float elapsedTime = 0f;
        Vector3 originalScale = transform.localScale;


        while (elapsedTime < shrinkTime)
        {
            float progress = elapsedTime / shrinkTime;
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, progress);
            dotRenderer.material.color = new Color(shrinkColor.r, shrinkColor.g, shrinkColor.b, 1 - progress); // Fade out
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
