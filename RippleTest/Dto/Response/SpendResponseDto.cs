namespace RippleTest.Dto;

public class SpendResponseDto
{
    public SpendResponseDto(bool finished, string? hash = null, string? url = null, string? errors = null)
    {
        Finished = finished;
        Hash = hash;
        Url = url;
        Errors = errors;
    }

    public bool Finished { get; set; }
    public string? Hash { get; set; }
    public string? Url { get; set; }
    public string? Errors { get; set; }
}