using API.Interfaces;
using API.Requests;
using API.Responses;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace API.Services
{
    public class SignupService : ISignupService
    {
        private readonly AppSettings appSettings;

        public SignupService(IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }

        public async Task<SignupResponse> Signup(SignupRequest signupRequest)
        {

            var dictionary = new Dictionary<string, string>
                    {
                        { "secret", appSettings.RecaptchaSecretKey },
                        { "response", signupRequest.ReCaptchaToken }
                    };

            var postContent = new FormUrlEncodedContent(dictionary);

            HttpResponseMessage recaptchaResponse = null;
            string stringContent = "";

            // Call recaptcha api and validate the token
            using (var http = new HttpClient())
            {
                recaptchaResponse = await http.PostAsync("https://www.google.com/recaptcha/api/siteverify", postContent);
                stringContent = await recaptchaResponse.Content.ReadAsStringAsync();
            }

            if (!recaptchaResponse.IsSuccessStatusCode)
            {
                return new SignupResponse() { Success = false, Error = "Unable to verify recaptcha token", ErrorCode = "S03" };
            }

            if (string.IsNullOrEmpty(stringContent))
            {
                return new SignupResponse() { Success = false, Error = "Invalid reCAPTCHA verification response", ErrorCode = "S04" };
            }

            var googleReCaptchaResponse = JsonConvert.DeserializeObject<ReCaptchaResponse>(stringContent);

            if (!googleReCaptchaResponse.Success)
            {
                var errors = string.Join(",", googleReCaptchaResponse.ErrorCodes);

                return new SignupResponse() { Success = false, Error = errors, ErrorCode = "S05" };
            }

            if (!googleReCaptchaResponse.Action.Equals("signup", StringComparison.OrdinalIgnoreCase))
            {
                // This is important just to verify that the exact action has been performed from the UI
                return new SignupResponse() { Success = false, Error = "Invalid action", ErrorCode = "S06" };
            }

            // Captcha was success , let's check the score, in our case, for example, anything less than 0.5 is considered as a bot user which we would not allow ...
            // the passing score might be higher or lower according to the sensitivity of your action

            if (googleReCaptchaResponse.Score < 0.5)
            {
                return new SignupResponse() { Success = false, Error = "This is a potential bot. Signup request rejected", ErrorCode = "S07" };
            }

            //TODO: Continue with doing the actual signup process, since now we know the request was done by potentially really human

            return new SignupResponse() { Success = true };
        }
    }
}
