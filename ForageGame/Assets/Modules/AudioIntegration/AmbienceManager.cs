using UnityEngine;
using System.Collections.Generic;
using System;

public class AmbienceManager : MonoBehaviour
{
    //This is to be a singleton, so we need to re-use this thing when we change regions!
    public static AmbienceManager Instance { get; private set; }

    //list of themes that can be played rn, depending on what trigger regions we are in
    private Dictionary<string, FMOD.Studio.PARAMETER_ID> parameters = new Dictionary<string, FMOD.Studio.PARAMETER_ID>();

    //Yes, I need all three. Thanks FMOD ~Lars
    [SerializeField] private FMODUnity.EventReference ambienceEvent;

    private FMOD.Studio.EventDescription ambienceEventDescription; 
    private FMOD.Studio.EventInstance ambienceEventInstance; 

    private void Awake() 
    { 
        //May only be one instance ofc
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }
    private void Start()
    {
        ambienceEventInstance = FMODUnity.RuntimeManager.CreateInstance(ambienceEvent);
        ambienceEventInstance.getDescription(out ambienceEventDescription);

        //TODO: this is debug. It should be done uuhhh with walkover triggers or something ~Lars
        ambienceEventInstance.start();

        //Populate the dictionary with parameters
        ambienceEventDescription.getParameterDescriptionCount(out int paramcount);
        for (int i = 0; i < paramcount; i++)
        {
            ambienceEventDescription.getParameterDescriptionByIndex(i, out var param);
            parameters.Add(param.name, param.id);
            print("Dict entry added: " + param.name + " with Id: " + param.id);
        }
    }

    public void SetParameter(string name, float value)
    {
        if (parameters.ContainsKey(name))
        {
            ambienceEventInstance.setParameterByID(parameters[name], value);
        }
        else
        {
            Debug.LogError("Recieved parameter name: " + name + " does not exist in current event");
        }
    }

}
