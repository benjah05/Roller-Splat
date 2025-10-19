using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BallController : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 15;

    public bool isTraveling;
    private Vector3 travelDirection;
    private Vector3 nextCollisionPosition;
    public int minSwipeRecognition = 500;
    private Vector2 swipePosLastFrame;
    private Vector2 swipePosCurrentFrame;
    private Vector2 currentSwipe;
    private Color solveColor;

    // public ParticleSystem rollingParticle;
    public AudioClip hittingWallSound;

    private void Start()
    {
        solveColor = Random.ColorHSV(0.5f, 1);
        GetComponent<MeshRenderer>().material.color = solveColor;

        // SetParticleColor(solveColor);

    }

    private void FixedUpdate()
    {
        if (isTraveling)
        {
            rb.velocity = speed * travelDirection;
        }


        Collider[] hitColliders = Physics.OverlapSphere(transform.position - (Vector3.up/2), .2f);
        int i = 0;

        while ( i < hitColliders.Length)
        {
            GroundPiece ground = hitColliders[i].transform.GetComponent<GroundPiece>();
            if (ground && !ground.isColored)
            {
                ground.ChangeColor(solveColor);
            }
            i++;
        }

        if (nextCollisionPosition != Vector3.zero)
        {
            if (Vector3.Distance(transform.position, nextCollisionPosition) < 1)
            {
                isTraveling = false;
                // rollingParticle.Stop();
                travelDirection = Vector3.zero;
                nextCollisionPosition = Vector3.zero;
            }
        }

        if (isTraveling)
            return;

        if (Input.GetMouseButton(0))
        {
            swipePosCurrentFrame = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            if (swipePosLastFrame != Vector2.zero)
            {
                currentSwipe = swipePosCurrentFrame - swipePosLastFrame;

                if (currentSwipe.sqrMagnitude < minSwipeRecognition)
                {
                    return;
                }

                currentSwipe.Normalize();

                // up or down
                if (currentSwipe.x > -0.5f && currentSwipe.x < 0.5)
                {
                    // go up or down
                    SetDestination(currentSwipe.y > 0 ? Vector3.forward : Vector3.back);
                }

                // up or down
                if (currentSwipe.y > -0.5f && currentSwipe.y < 0.5)
                {
                    // go left or right
                    SetDestination(currentSwipe.x > 0 ? Vector3.right : Vector3.left);
                }
            }

            swipePosLastFrame = swipePosCurrentFrame;
        }

        if (Input.GetMouseButtonUp(0))
        {
            swipePosLastFrame = Vector2.zero;
            currentSwipe = Vector2.zero;
        }
    }

    private void SetDestination(Vector3 direction)
    {
        travelDirection = direction;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, 100f))
        {
            nextCollisionPosition = hit.point;
        }

        isTraveling = true;
        // rollingParticle.Play();
        // Debug.Log("Particle Play Status: isPlaying = " + rollingParticle.isPlaying + 
        //           ", isEmitting = " + rollingParticle.isEmitting);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AudioSource.PlayClipAtPoint(hittingWallSound, transform.position);
        }
    }

    // private void SetParticleColor(Color newColor)
    // {
    //     var main = rollingParticle.main;
    //     main.startColor = newColor;
    // }
}