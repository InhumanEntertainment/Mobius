//============================================================================================================================================================================================//
// Ripped from: http://answers.unity3d.com/questions/32753/ideas-for-animating-while-paused.html //
//============================================================================================================================================================================================//
    
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animation))]
public class MenuAnimation : MonoBehaviour
{
    //for calculating our delta time
    float LastTime = 0F;
    float CurrentTime = 0F;
    float DeltaTime = 0F;

    AnimationState currentState;

    public bool IsPlaying;
    //float StartTime = 0F;
    public float AnimTime = 0F;

    public GameObject MessageObject;
    public string State;

    //============================================================================================================================================================================================//
    void Awake()
    {
        Update();
    }

    //============================================================================================================================================================================================//
    void Update()
    {
        CurrentTime = Time.realtimeSinceStartup;
        DeltaTime = CurrentTime - LastTime;
        LastTime = CurrentTime;

        if (IsPlaying) 
            AnimationUpdate();
    }

    //============================================================================================================================================================================================//
    void AnimationUpdate()
    {       
        AnimTime += DeltaTime;
        currentState.enabled = true;
        currentState.normalizedTime = AnimTime / currentState.length;
        
        if (AnimTime > currentState.length)
        {
            IsPlaying = false;
            OnAnimationCompleted();           
        }
    }

    //============================================================================================================================================================================================//
    public void Play(string name)
    {
        //print("Menu Animation: " + name);
        if (name != "")
        {
            print(animation);
            currentState = animation[name];
            currentState.weight = 1;
            currentState.blendMode = AnimationBlendMode.Blend;
            currentState.wrapMode = WrapMode.Once;
            currentState.normalizedTime = 0;
            currentState.enabled = true;

            AnimTime = 0F;
            Update();
            IsPlaying = true;
        }
    }

    //============================================================================================================================================================================================//
    internal void OnAnimationCompleted()
    {
        print("Menu Animation: " + currentState.name + " Completed");
        SendMessage(currentState.name, this, SendMessageOptions.DontRequireReceiver);
    }
}
