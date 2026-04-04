using UnityEngine;

public class StoryFlagWall : MonoBehaviour
{
    [SerializeField] private StoryFlag flag;

    void Awake()
    {
        StoryFlagManager.onFlagAdded += onStoryFlagAdded;
    }

    private void onStoryFlagAdded(StoryFlag newFlag)
    {
        if (newFlag == flag)
        {
            Destroy(gameObject);
        }
    }
}
