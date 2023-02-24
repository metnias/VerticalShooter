using UnityEngine;

public class Pool_Manager : MonoBehaviour
{
    public int count = 10; // pool instance counter
    public GameObject prefab; // prefab to fill the pool

    private int nextIndex; // next index to use
    private GameObject[] instances; // instances array


    private void Start()
    {
        FillPool();
    }

    private void FillPool()
    {
        instances = new GameObject[count];
        for (int i = 0; i < count; i++)
        {
            instances[i] = Instantiate(prefab, transform);
            instances[i].SetActive(false);
        }
        nextIndex = 0;
    }

    public bool TryDequeue(out GameObject instance)
    {
        instance = null;
        for (int fail = 0; fail < count; fail++)
        {
            int curIndex = nextIndex;
            nextIndex++;
            if (nextIndex >= count) nextIndex = 0; // tick nextIndex
            if (instances[curIndex] == null)
                instances[curIndex] = Instantiate(prefab, transform); // failsafe in case this got destroyed
            else if (instances[curIndex].activeSelf) continue; // already active. retry with next one.
            else instances[curIndex].SetActive(true);
            instance = instances[curIndex];
            return true;
        }
        return false;
    }
}
