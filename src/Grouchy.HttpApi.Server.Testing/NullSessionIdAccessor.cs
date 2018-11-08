using Grouchy.Abstractions.Tagging;

namespace Grouchy.HttpApi.Server.Testing
{
   public class NullSessionIdAccessor : ISessionIdAccessor
   {
      public string SessionId { get; } = null;
   }
}