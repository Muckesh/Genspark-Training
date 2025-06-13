public class AgentQueryDto
{
    public string? Name { get; set; }
    public string? AgencyName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }

    public string? SortBy { get; set; } = "name";
    public bool IsDescending { get; set; } = false;

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
