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
        // Obrót S1 wokół własnej osi
        RotateUsingMatrix(S1, rotationSpeedS1);

        // Obieg S2 wokół S1, z zachowaniem orientacji względem S1
        OrbitAndAlignLongSide(S2, S1, ref currentAngleS2, orbitRadiusS2, orbitSpeedS2);

        // Obieg S3 wokół S2, z zachowaniem orientacji względem S2
        OrbitAndAlignLongSide(S3, S2, ref currentAngleS3, orbitRadiusS3, orbitSpeedS3);
    }

    void RotateUsingMatrix(Transform obj, float speed)
    {
        float angle = speed * Time.deltaTime;
        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, angle));
        obj.position = rotationMatrix.MultiplyPoint3x4(obj.position - obj.position) + obj.position;
        obj.rotation *= Quaternion.Euler(0, 0, angle);
    }

    void OrbitAndAlignLongSide(Transform obj, Transform center, ref float currentAngle, float radius, float speed)
    {
        currentAngle += speed * Time.deltaTime;
        currentAngle %= 360f;

        float radian = currentAngle * Mathf.Deg2Rad;
        Vector3 localOffset = new Vector3(
            Mathf.Cos(radian) * radius,
            Mathf.Sin(radian) * radius,
            0
        );

        obj.position = center.position + localOffset;
        obj.rotation = center.rotation * Quaternion.Euler(0, 0, 90f); // Zachowanie orientacji
    }

    private void OnValidate()
    {
        // Początkowy kąt orbity
        currentAngleS2 = 0f;
        currentAngleS3 = 0f;

        // Ustaw pozycję i orientację S2 względem S1 za pomocą macierzy
        if (S1 != null && S2 != null)
        {
            Matrix4x4 translationMatrix = Matrix4x4.Translate(S1.position);
            Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, currentAngleS2));
            Matrix4x4 orbitMatrix = translationMatrix * rotationMatrix;

            Vector3 localOffset = new Vector3(orbitRadiusS2, 0, 0);
            S2.position = orbitMatrix.MultiplyPoint3x4(localOffset);

            S2.rotation = S1.rotation * Quaternion.Euler(0, 0, 90f); // Zachowanie orientacji
        }

        // Ustaw pozycję i orientację S3 względem S2 za pomocą macierzy
        if (S2 != null && S3 != null)
        {
            Matrix4x4 translationMatrix = Matrix4x4.Translate(S2.position);
            Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, currentAngleS3));
            Matrix4x4 orbitMatrix = translationMatrix * rotationMatrix;

            Vector3 localOffset = new Vector3(orbitRadiusS3, 0, 0);
            S3.position = orbitMatrix.MultiplyPoint3x4(localOffset);

            S3.rotation = S2.rotation * Quaternion.Euler(0, 0, 90f); // Zachowanie orientacji
        }
    }
}
