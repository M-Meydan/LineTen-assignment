namespace Application.Common.Interfaces.Infrastructure.DI
{
    /// <summary>
    /// Three interfaces to mark our dependencies with different lifetimes automatically with Scrutor
    /// </summary>
    public interface ISingletonDependency { }

    public interface ITransientDependency { }

    public interface IScopedDependency { }
}
