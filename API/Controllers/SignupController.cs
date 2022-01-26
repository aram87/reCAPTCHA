using API.Interfaces;
using API.Requests;
using API.Responses;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignupController : ControllerBase
    {
        private readonly ISignupService signupService;

        public SignupController(ISignupService signupService)
        {
            this.signupService = signupService;
        }

        [HttpPost]
        public async Task<IActionResult> Signup(SignupRequest signup)
        {
            if (signup == null)
            {
                return BadRequest(new SignupResponse() { Success = false, Error = "Invalid signup request", ErrorCode = "S01" });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors.Select(c => c.ErrorMessage)).ToList();
                if (errors.Any())
                {
                    return BadRequest(new SignupResponse
                    {
                        Error = $"{string.Join(",", errors)}",
                        ErrorCode = "S02"
                    });
                }
            }

            var response = await signupService.Signup(signup);

            if (!response.Success)
            {
                return UnprocessableEntity(response);
            }

            return Ok(new SignupResponse() { Success = true });

        }
    }
}
