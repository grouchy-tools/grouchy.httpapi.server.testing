using Grouchy.HttpApi.Server.Abstractions.EventCallbacks;
using Grouchy.HttpApi.Server.Abstractions.Events;

namespace Grouchy.HttpApi.Server.Testing
{
   public class NoOpHttpServerEventCallback : IHttpServerEventCallback
   {
      public void Invoke(IHttpServerEvent @event)
      {
      }
   }
}
