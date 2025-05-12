using TournamentManager.Core.Entities;

namespace TournamentManager.Core.Events;

public class SignedInEvent : PubSubEvent<User>
{
}
