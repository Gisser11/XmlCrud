using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WebApplication2.Models;

public class jwtModel
{
    public const string ISSUER = "MyAuthServer"; 
    public const string AUDIENCE = "MyAuthClient"; 
    public const string KEY = "mysupersecret_secretkey!1234"; 
    public const int LIFETIME = 1; 
    public static SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
    }
}