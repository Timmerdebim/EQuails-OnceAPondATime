using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class GibberishSpeaker : MonoBehaviour
{
    [SerializeField] private NpcVoice profile;
    [SerializeField] private Transform speakerTransform; //for directional audio
    //ducking snapshot
    public FMODUnity.EventReference DuckAllForDialogueSnapshot;
    FMOD.Studio.EventInstance DuckAllForDialogue; 

    private int remaining;
    private float timer;


    public void DialogueOpened()
    {
        if (DuckAllForDialogue.isValid())  DuckAllForDialogue.start(); //Duck any playing ambience or theme - focus is on the dialogue
    }

    public void DialogueClosed()
    {
        if (DuckAllForDialogue.isValid())  DuckAllForDialogue.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); //Return the audio level
    }

    // Call on every new textbox. Replaces any speech still in progress.
    public void Speak(int syllables) 
    { 
        remaining = syllables; 
        timer = 0f;
    }

    private void Update()
    {
        if (remaining <= 0) return;
        timer -= Time.unscaledDeltaTime; //basically: unaffected by timescale
        if (timer > 0f) return;

        int i = Pick();
        var inst = RuntimeManager.CreateInstance(profile.voiceEvent);
        var speakerLoc = speakerTransform is null ? speakerTransform : transform;
        inst.set3DAttributes(RuntimeUtils.To3DAttributes(speakerLoc));
        inst.setParameterByNameWithLabel("Syllable", profile.syllableVariants[i].label);
        inst.start();
        inst.release();

        remaining--;
        timer += profile.syllableVariants[i].clipLength * Random.Range(1f - profile.jitter, 1f + profile.jitter);
    }

    //random choic from weighted list
    private int Pick()
    {
        float r = Random.value; //remember weights sum to 1

        for (int i = 0; i < profile.syllableVariants.Length; i++)
        {
            r -= profile.syllableVariants[i].weight;
            if (r <= 0f) return i;
        }
        Debug.LogError($"[GibberishSpeaker]: no syllable variant selected? check voice profile weights: {profile.name}");
        return 0;
    }
}