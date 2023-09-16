namespace Epica.Web.Operacion.Helpers
{
    public class UserContextService
    {
        private LoginResponse _loginResponse;
        private TokenResponse _tokenResponse;

        public void SetLoginResponse(LoginResponse loginResponse)
        {
            _loginResponse = loginResponse;
        }

        public void SetTokenResponse(TokenResponse tokenResponse)
        {
            _tokenResponse = tokenResponse;
        }

        public LoginResponse GetLoginResponse()
        {
            return _loginResponse;
        }
        public TokenResponse GetTokenResponse()
        {
            return _tokenResponse;
        }

    }
}
