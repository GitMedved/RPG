namespace MiaMax.Gameplay
{
    public interface IBackyardInteractable
    {
        string DisplayName { get; }
        string ActionLabel { get; }
        void Interact();
    }
}
