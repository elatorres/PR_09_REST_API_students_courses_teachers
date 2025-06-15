namespace ServerRestApi_SCT.Models;
// This is one of ours DTOs (Data Transfer Objects)
public class Course
{
    public int CId { get; set; } // Unique identifier for each Course
    public string Name { get; set; } = string.Empty; // Course’s name
    public string Department { get; set; } = string.Empty; // Course’s Department
    public string Description { get; set; } // Course’s Description
}