using System.Xml.Serialization;

namespace WebApplication2.Models;


[XmlRoot("Reports")]
public class Reports
{
    [XmlElement("Data")]
    public List<RequestModel> DataList { get; set; }
}
public class RequestModel
{
    public int ReportId { get; set; }
    
    public string? Server { get; set; }
    
    public string? DataBase { get; set; }
    
    public string? UserName { get; set; }
    
    public string? UserPassword { get; set; }
    
    public int? DataSourceType { get; set; }
    
    public string? FileQueryColName { get; set; }
    
    public string? FileQueryOutputColName { get; set; }
    
    public string? FileQueryTempPath { get; set; }
    
    public string? UsePivotData { get; set; }
    
    public string? PivotCol { get; set; }
    
    public string? PivotCols { get; set; }
    
    public string? PivotRows { get; set; }
    
    public string? PivotData { get; set; }
    
    public string? DefaultComplex { get; set; }
}
