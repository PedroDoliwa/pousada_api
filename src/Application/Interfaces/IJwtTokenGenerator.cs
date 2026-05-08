using PousadaApi.Domain.Entities;

namespace PousadaApi.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string Generate(Usuario usuario);
}
