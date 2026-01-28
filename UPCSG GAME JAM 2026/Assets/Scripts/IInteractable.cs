using UnityEngine;

public interface IInteractable
{
    void Interact(MonoBehaviour name);
    bool CanInteract(); 
}