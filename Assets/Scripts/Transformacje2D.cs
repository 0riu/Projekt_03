using UnityEngine;

public class Transformacje2D : MonoBehaviour
{
    public Transform S1; // Żółty kwadrat
    public Transform S2; // Zielony prostokąt
    public Transform S3; // Niebieski prostokąt

    [SerializeField] private float rotationSpeedS1 = 50f; // Prędkość obrotu S1
    [SerializeField] private float orbitSpeedS2 = 50f; // Prędkość obiegu S2 wokół S1
    [SerializeField] private float orbitSpeedS3 = 50f; // Prędkość obiegu S3 wokół S2
    [SerializeField] private float orbitRadiusS2 = 3f; // Promień orbity S2 wokół S1
    [SerializeField] private float orbitRadiusS3 = 1.5f; // Promień orbity S3 wokół S2

    private float currentAngleS2 = 0f; // Aktualny kąt S2 w orbicie
    private float currentAngleS3 = 0f; // Aktualny kąt S3 w orbicie

    void Update()
    {
        // Obrót S1 wokół własnej osi za pomocą macierzy
        RotateUsingMatrix(S1, rotationSpeedS1);

        // Obieg S2 wokół S1 za pomocą macierzy
        OrbitAndAlignUsingMatrix(S2, S1, ref currentAngleS2, orbitRadiusS2, orbitSpeedS2);

        // Obieg S3 wokół S2 za pomocą macierzy
        OrbitAndAlignUsingMatrix(S3, S2, ref currentAngleS3, orbitRadiusS3, orbitSpeedS3);
    }

    void RotateUsingMatrix(Transform obj, float speed)
    {
        // Kąt obrotu dla bieżącej klatki
        float angle = speed * Time.deltaTime;

        // Macierz rotacji
        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, angle));

        // Obrót obiektu (pozycja i orientacja)
        obj.position = rotationMatrix.MultiplyPoint3x4(obj.position - obj.position) + obj.position;
        obj.rotation *= Quaternion.Euler(0, 0, angle);
    }

    void OrbitAndAlignUsingMatrix(Transform obj, Transform center, ref float currentAngle, float radius, float speed)
    {
        // Aktualizacja kąta orbity
        currentAngle += speed * Time.deltaTime;
        currentAngle %= 360f;

        // Tworzenie macierzy transformacji dla orbity
        Matrix4x4 translationMatrix = Matrix4x4.Translate(center.position);
        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, currentAngle));
        Matrix4x4 orbitMatrix = translationMatrix * rotationMatrix;

        // Pozycja na orbicie
        Vector3 localOffset = new Vector3(radius, 0, 0);
        Vector3 newPosition = orbitMatrix.MultiplyPoint3x4(localOffset);

        // Ustawienie pozycji
        obj.position = newPosition;

        // Zachowanie orientacji względem centrum
        obj.rotation = center.rotation * Quaternion.Euler(0, 0, 90f);
    }

    private void OnValidate()
    {
        // Ustawienie pozycji i orientacji S2 względem S1 za pomocą macierzy
        if (S1 != null && S2 != null)
        {
            Matrix4x4 translationMatrix = Matrix4x4.Translate(S1.position);
            Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 0)); // Początkowy kąt
            Matrix4x4 orbitMatrix = translationMatrix * rotationMatrix;

            Vector3 localOffset = new Vector3(orbitRadiusS2, 0, 0);
            S2.position = orbitMatrix.MultiplyPoint3x4(localOffset);

            S2.rotation = S1.rotation * Quaternion.Euler(0, 0, 90f);
        }

        // Ustawienie pozycji i orientacji S3 względem S2 za pomocą macierzy
        if (S2 != null && S3 != null)
        {
            Matrix4x4 translationMatrix = Matrix4x4.Translate(S2.position);
            Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 0)); // Początkowy kąt
            Matrix4x4 orbitMatrix = translationMatrix * rotationMatrix;

            Vector3 localOffset = new Vector3(orbitRadiusS3, 0, 0);
            S3.position = orbitMatrix.MultiplyPoint3x4(localOffset);

            S3.rotation = S2.rotation * Quaternion.Euler(0, 0, 90f);
        }
    }
}
