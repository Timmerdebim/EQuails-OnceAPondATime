using UnityEngine;


/// <summary>
/// This is a bit of a roundabout way to do this,
/// But I have no idea how gadgets work
/// Thus this is my epic hack to still have saving and loading work
/// ~Lars
/// 
/// 
/// TODO: THIS IS TO BE OVERHAULED FOR THE NEW PRINCESS VISUALS
/// </summary>

public class FlowerSpotFlagListener : MonoBehaviour
{
    [SerializeField] private StoryFlag SoilPlacedFlag;
    [SerializeField] private StoryFlag FlowerPlantedFlag;

    [SerializeField] private Animator animator;

    void OnEnable()
    {
        StoryFlagManager.onFlagAdded += onStoryFlagAdded;
        StoryFlagManager.onStoryFlagsLoaded += OnStoryFlagsChanged;
    }
    void OnDisable()
    {
        StoryFlagManager.onFlagAdded -= onStoryFlagAdded;
        StoryFlagManager.onStoryFlagsLoaded -= OnStoryFlagsChanged;
    }

    private void onStoryFlagAdded(StoryFlag newFlag)
    {
        if (newFlag == FlowerPlantedFlag)
        {
            animator.SetBool("FlowerPlanted", true);
        }
        else if (newFlag == SoilPlacedFlag)
        {
            animator.SetBool("SoilPlaced", true);
        }
    }

    private void OnStoryFlagsChanged()
    {
        if (StoryFlagManager.Instance.FlagActive(FlowerPlantedFlag))
        {
            animator.SetBool("FlowerPlanted", true);
        }
        if (StoryFlagManager.Instance.FlagActive(FlowerPlantedFlag))
        {
            animator.SetBool("SoilPlaced", true);
        }
    }
}
