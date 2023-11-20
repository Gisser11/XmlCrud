using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using WebApplication2.Models;
using WebApplication2.Models.Constants;
using Formatting = Newtonsoft.Json.Formatting;

namespace WebApplication2.Controllers;


[ApiController]
[Route("xml")]
public class XmlController : Controller
{
    
    private IWebHostEnvironment Environment;
    public string doc;
    public bool ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(jwtModel.KEY); 

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = jwtModel.ISSUER,
                ValidAudience = jwtModel.AUDIENCE,
                ValidateLifetime = false  
            }, out SecurityToken validatedToken);

             
            if (validatedToken is JwtSecurityToken jwtToken)
            {
                 
                var expires = jwtToken.ValidTo;
                 
                if (expires < DateTime.UtcNow)
                {
                    return false;  
                }
            }

            return true;  
        }
        catch
        {
            return false;  
        }
    }
    public string GetXML()
    {
        var xmlFilePath = Environment.ContentRootPath + Constants.PATH + Constants.XML;
        var xmlData = System.IO.File.ReadAllText(xmlFilePath);
        return xmlData;
    }

    public XmlController(IWebHostEnvironment environment)
    {
        Environment = environment;
        doc = GetXML();
    }

    [HttpGet("api/Reports")]
    public IActionResult Reports()
    {
        string myCookie = Request.Cookies["token"];
        
        if (ValidateToken(myCookie)) {
            XmlSerializer serializer = new XmlSerializer(typeof(Reports));
        
        
            using (StringReader reader = new StringReader(doc))
            {   
                Reports reports = (Reports)serializer.Deserialize(reader);
                string jsonData = JsonConvert.SerializeObject(reports, Formatting.Indented);
                return Ok(jsonData);
            }
        }
        if (!ValidateToken(myCookie))
        {
            Response.Cookies.Delete("token");
        }
        return StatusCode(404,"Обновите страницу");
    }
    
    
    [Route("DeleteNode/{ReportId:int}")]
    public IActionResult DeleteNode([FromRoute] int ReportId)
    {
        string myCookie = Request.Cookies["token"];

        if (ReportId == 0)
        {
            return StatusCode(404);
        }
        
        if (ValidateToken(myCookie))
        {
            XmlDocument document = new XmlDocument();
            document.Load(string.Concat(this.Environment.ContentRootPath, Constants.PATH, Constants.XML));
        
            foreach (XmlNode node in document.SelectNodes(Constants.PARENT_NODE))
            {
                if (ReportId == int.Parse(node["ReportId"].InnerText))
                {
                    XmlNode parent = node.ParentNode;
                    parent.RemoveChild(node);
                    document.Save(string.Concat(this.Environment.ContentRootPath, "/Services", "/RQList.xml"));
                    return Ok();
                
                }
            }
            return NoContent();
        }
        return StatusCode(404, "Произошла ошибка, обратитесь куда-нибудь");
    }
    
    [Route("api/EditNode")]
    [HttpPut]
    public IActionResult EditNode([FromBody] RequestModel updatedModel)
    {
        string myCookie = Request.Cookies["token"];

        if (updatedModel.ReportId == 0)
        {
            return StatusCode(404);
        }

        if (ValidateToken(myCookie))
        {
            if (ModelState.IsValid)
            {
                XmlDocument document = new XmlDocument();
                document.Load(string.Concat(this.Environment.ContentRootPath, Constants.PATH, Constants.XML));
                foreach (XmlNode node in document.SelectNodes(Constants.PARENT_NODE))
                {
                    if (updatedModel.ReportId == int.Parse(node["ReportId"].InnerText))
                    {
                        foreach (PropertyInfo prop in typeof(RequestModel).GetProperties())
                        {
                            string propName = prop.Name;
                            object propValue = prop.GetValue(updatedModel);
                        
                            if (propValue != null)
                            {
                                string nodeValue = Convert.ToString(propValue);

                                XmlNode targetNode = node[propName];
                                if (targetNode == null)
                                {
                                    targetNode = document.CreateElement(propName);
                                    node.AppendChild(targetNode);
                                }

                                targetNode.InnerText = nodeValue;
                            }
                        
                            document.Save(string.Concat(this.Environment.ContentRootPath, "/Services", "/RQList.xml"));
                        }
                    }
                }
                return Ok(updatedModel);
            }
        }
        else
        {
            Response.Cookies.Delete("token");
        }
        return Unauthorized();
    }

}