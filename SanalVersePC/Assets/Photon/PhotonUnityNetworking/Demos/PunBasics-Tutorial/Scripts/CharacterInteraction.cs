using UnityEngine;

public class CharacterInteraction : MonoBehaviour
{
    public float walkingSpeed = 2f; // Y�r�me h�z�
    public Transform sittingPosition; // Oturma pozisyonu
    private Animator animator; // Animator referans�
    private bool isWalking = false; // Y�r�me durumu
    private bool isSitting = false; // Oturma durumu

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isWalking)
        {
            // Hedefe do�ru y�r�me i�lemi
            Vector3 targetDirection = sittingPosition.position - transform.position;
            targetDirection.y = 0f; // Y y�n�nde hareket etmesini engelle
            transform.rotation = Quaternion.LookRotation(targetDirection);
            transform.position += targetDirection.normalized * walkingSpeed * Time.deltaTime;

            // Hedefe yakla�t���nda oturma animasyonunu oynat
            if (Vector3.Distance(transform.position, sittingPosition.position) <= 0.1f)
            {
                isWalking = false;
                isSitting = true;
                animator.SetBool("IsSitting", true); // Oturma animasyonunu ba�lat
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Oturabilir") && !isWalking && !isSitting)
        {
            isWalking = true;
        }
    }
}