using UnityEngine;

public class RandomLocalPassword : MonoBehaviour
{
    private float inicialXRange = -9f;
    private float finalXRange = 9f;

    private float inicialZRange = -6f;
    private float finalZRange = 6f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RandomLocal();
    }

    private void RandomLocal()
    {
        float xNumber = Random.Range(inicialXRange, finalXRange);
        float zNumber = Random.Range(inicialZRange, finalZRange);

        Debug.Log(xNumber);
        Debug.Log(zNumber);

        gameObject.transform.position = new Vector3(xNumber, gameObject.transform.position.y, zNumber);
    }

    
}
