using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Company.Services.Identity.Shared.Responses;

public class ProblemResponse : ResponseBase
{                
    public string? Type { get; set; }      
    
    public string? Title { get; set; }        
 
    public string? Detail { get; set; }     
 
    public string? Instance { get; set; }
 
    [JsonExtensionData]
    public IDictionary<string, object?> Extensions { get; } = new Dictionary<string, object?>(StringComparer.Ordinal);

    public ProblemResponse() : base()
    {
    }

    public ProblemResponse(string error) : base(500, error)
    {            
    }
}
