using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

//The following code is heavily influenced from a state machine used in Side By Side (Producer: Yoon Lee)

/// <summary>
/// Defines a Finite State Machine that can be extended for more functionality.
/// Don't mess with this code; if you're confused about how to implement an FSM, @me in the discord.
/// </summary>
/// <typeparam name="S">An abstract class that defines what kind of state you want</typeparam>
/// <typeparam name="I">A class that carries values between states</typeparam>
public abstract class StateMachine<M, S, I> : MonoBehaviour
    where M : StateMachine<M, S, I>
    where S : State<M, S, I>
    where I : StateInput
{
    public Dictionary<Type, S> StateMap { get; private set; }
    public S CurState { get; protected set; }
    public I CurInput { get; private set; }

    protected void SetCurState<T>() where T : S {
        if (StateMap.ContainsKey(typeof(T))) {
            CurState = StateMap[typeof(T)];
        } else {
            Debug.LogError("Error: state machine doesn't include type " + typeof(T));
            Debug.Break();
        }
    }

    void InitStateInput() { CurInput = (I) Activator.CreateInstance(typeof(I)); }

    // Start is called before the first frame update
    void Start() {
        InitStateInput();
        
        //The below code was provided by Side By Side (Producer: Yoon Lee), who got it from Brandon Shockley
        //Gets all inherited classes of S and instantiates them using voodoo magic code I got from Brandon Shockley lol
        StateMap = new Dictionary<Type, S>();
        // loadedStates = new GenericDictionary<string, string>();
        Init();
        foreach (Type type in Assembly.GetAssembly(typeof(S)).GetTypes()
            .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(S)))) {
            S newState = (S) Activator.CreateInstance(type);
            newState.MySM = (M) this;
            // newState.character = this;
            // newState.Init(stateInput);
            StateMap.Add(type, newState);
            // loadedStates.Add(type.FullName, RuntimeHelpers.GetHashCode(newState).ToString());
        }
        
        SetInitialState();
    }

    protected void Update() {
        CurState.Update(CurInput);
    }

    public void Transition<NextStateType>() where NextStateType : S {
        CurState.Exit(CurInput);
        SetCurState<NextStateType>();
        CurState.Enter(CurInput);
    }

    public bool IsOnState<CheckStateType>() where CheckStateType : S{
        return CurState.GetType() == typeof(CheckStateType);
    }

    public virtual void Init() { }
    protected abstract void SetInitialState();
}

public abstract class State <M, S, I> 
    where M : StateMachine<M, S, I> 
    where S : State<M, S, I> 
    where I : StateInput
{
    public M MySM;

    public virtual void Enter(I i) { }
    public virtual void Exit(I i) { }

    public virtual void Update(I i) { }

    public void Transition() { }
}

public class StateInput {
}