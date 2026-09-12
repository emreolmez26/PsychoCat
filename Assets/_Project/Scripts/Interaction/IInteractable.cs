namespace PsychoCat.Interaction
{
    public interface IInteractable
    {
        string InteractionPrompt { get; }
        void Interact(UnityEngine.GameObject interactor);
    }
}
