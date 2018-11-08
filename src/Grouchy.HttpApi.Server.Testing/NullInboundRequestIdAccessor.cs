using Grouchy.HttpApi.Server.Abstractions.Tagging;

namespace Grouchy.HttpApi.Server.Testing
{
   public class NullInboundRequestIdAccessor : IInboundRequestIdAccessor
   {
      public string InboundRequestId { get; } = null;
   }
}