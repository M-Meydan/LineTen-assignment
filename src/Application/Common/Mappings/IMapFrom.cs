using AutoMapper;

namespace Application.Common.Mappings;

/// <summary>
/// Used for adding dynamic mapping from a type at start up
/// </summary>
public interface IMapFrom<T>
{
    void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType());
}
