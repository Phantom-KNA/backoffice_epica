namespace Epica.Api.Operacion.Config;

public class UrlsConfig
{
	public class TokenOperations
	{
		public static string GetToken() => $"/api/login";
	}

	public string Token { get; set; }
}
