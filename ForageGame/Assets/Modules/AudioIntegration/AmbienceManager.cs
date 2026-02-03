using UnityEngine;
using System.Collections.Generic;
using System;

public class AmbienceManager : MonoBehaviour
{
    //This is to be a singleton, so we need to re-use this thing when we change regions!
    public static AmbienceManager Instance { get; private set; }

    [System.Serializable]
    public class AmbienceEvent
    {
        public bool active = false; //is this event playing
        public FMOD.Studio.EventInstance instance; 
        public Dictionary<string, FMOD.Studio.PARAMETER_ID> parameters = new Dictionary<string, FMOD.Studio.PARAMETER_ID>();
    }

    [SerializeField] private List<Region> regions;

    //kinda lame to *also* have this, but I want to edit the list above in the editor, and ofc I can't serialize a dictionary
    private Dictionary<Region, AmbienceEvent> events = new Dictionary<Region, AmbienceEvent>();

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
        foreach(Region r in regions)
        {
            if(events.ContainsKey(r)) Debug.LogError($"Duplicate AmbienceRegion Id: {r}");

            AmbienceEvent e = new AmbienceEvent();
            e.instance = FMODUnity.RuntimeManager.CreateInstance(r.eventReference);
            e.instance.getDescription(out FMOD.Studio.EventDescription desc);

            e.instance.start(); //TODO: this is debug
            e.active = true; //IDEM DITO

            desc.getParameterDescriptionCount(out int paramcount);
            for (int i = 0; i < paramcount; i++)
            {
                desc.getParameterDescriptionByIndex(i, out var param);
                e.parameters.Add(param.name, param.id);
                print("Dict entry added: " + param.name + " with Id: " + param.id + "for region: " + r);
            }
            events.Add(r, e);
        }
    }

    public void SetParameter(string param, float value)
    {
        foreach(AmbienceEvent e in events.Values)
        {
            if(e.active) //only check active events
            {
                if (e.parameters.TryGetValue(param, out var id))
                {
                    e.instance.setParameterByID(id, value);
                    return;
                }
            }
        }
        Debug.LogError("Recieved parameter name: " + name + " does not exist in an active ambience event");
    }
}

