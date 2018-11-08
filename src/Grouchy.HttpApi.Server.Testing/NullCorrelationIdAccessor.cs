using Grouchy.Abstractions.Tagging;

namespace Grouchy.HttpApi.Server.Testing
{
   public class NullCorrelationIdAccessor : ICorrelationIdAccessor
   {
      public string CorrelationId { get; } = null;
   }
}