using API.Requests;
using API.Responses;

namespace API.Interfaces
{
    public interface ISignupService
    {
        Task<SignupResponse> Signup(SignupRequest signupRequest);
    }
}
