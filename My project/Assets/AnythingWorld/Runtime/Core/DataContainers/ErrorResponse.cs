using System;
[Serializable]
public class ErrorResponse
{
    public string message;
    public string code;
    public ErrorResponse() { }
    public ErrorResponse(string _message, string _code = "")
    {
        message = _message;
        code = _code; 
    }
}