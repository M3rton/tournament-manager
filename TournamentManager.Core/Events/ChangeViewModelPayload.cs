namespace TournamentManager.Core.Events;

public class ChangeViewModelPayload
{
    public object Sender { get; set; }
    public string ViewModelName { get; set; }
}
