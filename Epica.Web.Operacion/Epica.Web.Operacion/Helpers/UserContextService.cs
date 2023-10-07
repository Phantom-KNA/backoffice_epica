namespace Epica.Web.Operacion.Helpers
{
    public class UserContextService
    {
        private LoginResponse _loginResponse;
        public void SetLoginResponse(LoginResponse loginResponse)
        {
            _loginResponse = loginResponse;
        }

        public LoginResponse GetLoginResponse()
        {
            return _loginResponse;
        }

    }
}
