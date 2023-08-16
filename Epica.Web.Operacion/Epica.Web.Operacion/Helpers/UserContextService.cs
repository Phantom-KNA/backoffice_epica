namespace Epica.Web.Operacion.Helpers
{
    public class UserContextService
    {
        private LoginResponse _loginResponse;
        private int _userId;

        public void SetLoginResponse(LoginResponse loginResponse)
        {
            _loginResponse = loginResponse;
        }

        public void SetUserId(int userId)
        {
            _userId = userId;
        }

        public LoginResponse GetLoginResponse()
        {
            return _loginResponse;
        }

        public int GetUserId()
        {
            return _userId;
        }
    }
}
