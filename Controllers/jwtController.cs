using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApplication2.Models;

namespace WebApplication2.Controllers;


public class jwtController : Controller
{

    public string GetToken()
    {
        var jwt = new JwtSecurityToken(
            issuer: jwtModel.ISSUER,
            audience: jwtModel.AUDIENCE,
            expires: DateTime.Today.AddDays(1),
            signingCredentials: new SigningCredentials(jwtModel.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
        return encodedJwt;
    }

    [HttpPost("/token")]
    public IActionResult token([FromBody] TokenRequest request)
    {
        if (jwtModel.KEY != request.jwtKey)
        {
            return BadRequest(new { errorText = "Invalid jwt Key" });
        }

        string token = GetToken();
        Response.Cookies.Append("token", token//, new CookieOptions
        //{
          //  HttpOnly = false // Вот это обязательно ли должно быть? 
            // Просто в ext js нужно как-то вытянуть куки, чтобы понять, рендерить ли модалку с авторизацией
        //}
        );
        
        var response = new
        {
            access_token = token,
        };
 
        return Json(response);
    }

    [HttpPost("/token/delete")]
    public IActionResult deleteToken()
    {
        Response.Cookies.Delete("token");
        return Ok("Успешно");
    }
}