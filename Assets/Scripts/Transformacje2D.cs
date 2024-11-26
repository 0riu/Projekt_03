using UnityEngine;

public class Transformacje2D : MonoBehaviour
{
    public Transform S1; // Kwadrat
    public Transform S2; // Prostokąt krążący wokół S1
    public Transform S3; // Prostokąt krążący wokół S2

    [SerializeField] private float rotationSpeedS1 = 50f; // Prędkość obrotu S1
    [SerializeField] private float orbitSpeedS2 = 45f; // Prędkość obiegu S2 wokół S1
    [SerializeField] private float orbitSpeedS3 = 150f; // Prędkość obiegu S3 wokół S2
    [SerializeField] private float orbitRadiusS2 = 3.5f; // Promień orbity S2 wokół S1
    [SerializeField] private float orbitRadiusS3 = 1.5f; // Promień orbity S3 wokół S2

    private float currentAngleS2 = 0f; // Aktualny kąt S2 w orbicie
    private float currentAngleS3 = 0f; // Aktualny kąt S3 w orbicie

    void Start()
    {
        // Ustawienie prędkości i promieni orbit przy uruchomieniu
        rotationSpeedS1 = 50f;
        orbitSpeedS2 = 45f;
        orbitSpeedS3 = 150f;
        orbitRadiusS2 = 3.5f;
        orbitRadiusS3 = 1.5f;
    }

    void Update()
    {
        // Obrót S1 wokół własnej osi za pomocą macierzy
        RotateObject(S1, rotationSpeedS1);

        // Obieg S2 wokół S1 za pomocą macierzy
        OrbitUsingMatrix(S2, S1.position, ref currentAngleS2, orbitRadiusS2, orbitSpeedS2);

        // Obieg S3 wokół S2 za pomocą macierzy
        OrbitUsingMatrix(S3, S2.position, ref currentAngleS3, orbitRadiusS3, orbitSpeedS3);
    }

    void RotateObject(Transform obj, float speed)
    {
        // Macierz rotacji
        float angle = speed * Time.deltaTime;
        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, angle));

        // Zastosowanie rotacji
        obj.position = rotationMatrix.MultiplyPoint3x4(obj.position);
        obj.rotation *= Quaternion.Euler(0, 0, angle);
    }

    void OrbitUsingMatrix(Transform obj, Vector3 center, ref float currentAngle, float radius, float speed)
    {
        // Aktualizacja kąta orbity
        currentAngle += speed * Time.deltaTime;
        currentAngle %= 360f;

        // Konwersja kąta na radiany
        float radian = currentAngle * Mathf.Deg2Rad;

        // Tworzenie macierzy transformacji
        Matrix4x4 translationMatrix = Matrix4x4.Translate(center);
        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, currentAngle));
        Matrix4x4 orbitMatrix = translationMatrix * rotationMatrix;

        // Obliczanie nowej pozycji
        Vector3 localOffset = new Vector3(radius, 0, 0); // Punkt na promieniu
        Vector3 newPosition = orbitMatrix.MultiplyPoint3x4(localOffset);

        // Ustawienie pozycji i zachowanie orientacji
        obj.position = newPosition;
        obj.rotation = Quaternion.identity;
    }

    private void OnValidate()
    {
        // Ustawienie pozycji S2
        if (S1 != null && S2 != null)
        {
            S2.position = S1.position + new Vector3(orbitRadiusS2, 0, 0);
        }

        // Ustawienie pozycji S3
        if (S2 != null && S3 != null)
        {
            S3.position = S2.position + new Vector3(orbitRadiusS3, 0, 0);
        }
    }
}
