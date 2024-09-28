using BlazorIdentityApp.Shared.Dto;
using static BlazorIdentityApp.Shared.Dto.ServiceResponses;

namespace BlazorIdentityApp.Shared.Interfaces;

public interface IUserAccount
{
    Task<GeneralResponse> CreateAccount(UserDto userDTO);
    Task<LoginResponse> LoginAccount(LoginDto loginDTO);
}
