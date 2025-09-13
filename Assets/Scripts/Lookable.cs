using UnityEngine;
using UnityEngine.InputSystem;

public class Lookable : MonoBehaviour
{
    public bool isVisible;
    public void Appear()
    {
        Debug.Log("ObjectAppear");
        //Make the object visible
        isVisible = true;
    }
    public void Disappear()
    {
        Debug.Log("ObjectDisappear");
        //Make the object unvisible
        isVisible = false;
    }
}