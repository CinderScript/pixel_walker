using System.Collections;
using System.Linq;

using UnityEngine;

public class SceneGuiInterface : MonoBehaviour
{
    [Header("Area of BehaviorController")]
    [Tooltip("This camera controller will use this PlayerArea to find the player to follow.")]
    public GameObject sceneArea;

	private BehaviorController behaviorController;
	private PropInfo[] props;

	// Start is called before the first frame update
	void Awake()
    {
        behaviorController = sceneArea.GetComponentInChildren<BehaviorController>();
    }

	private void Start()
	{
		props = sceneArea.GetComponentInChildren<AreaProps>().Props;
		
		var behavior = new AgentBehaviorProperties(BehaviorType.Navigate, "", "");
		SelectBehavior(behavior);
	}

	public void SelectBehavior(AgentBehaviorProperties properties)
	{
		behaviorController.StartBehavior(properties);
	}

	public void StopBehavior()
	{
		behaviorController.StopBehavior();
	}

	public string GetPropsList()
	{
		string[] names = props.Select(prop => prop.Name).ToArray();
		return string.Join(", ", names);
	}
}