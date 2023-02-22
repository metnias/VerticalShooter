using UnityEngine;

public class Pool_Manager : MonoBehaviour
{
    public int count = 10;
    public GameObject prefab;

    private int curIndex;
    private GameObject[] instances;


    private void Start()
    {
        FillPool();
    }

    private void FillPool()
    {
        instances = new GameObject[count];
        for(int i = 0; i < count; i++)
        {
            instances[i] = Instantiate(prefab, transform);
            instances[i].SetActive(false);
        }
        curIndex = 0;
    }

    public bool TryDequeue(out GameObject instance)
    {
        instance = null;
        int index = curIndex;
        if (instances[index] == null)
            instances[index] = Instantiate(prefab, transform);
        else if (instances[index].activeSelf) return false;
        else instances[index].SetActive(true);
        instance = instances[index];
        curIndex++;
        if (curIndex >= count) curIndex = 0;
        return true;
    }
}
