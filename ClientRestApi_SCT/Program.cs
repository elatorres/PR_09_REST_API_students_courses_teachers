using System.Net.Http.Json;

namespace ClientRestApi_SCT;
class Program
{
    static async Task Main()
    {
        string baseUrl = "http://localhost:5169/api/";
        // Create a client
        using var client = new HttpClient();
        // Connect to API Server 
        client.BaseAddress = new Uri(baseUrl);
        
        while (true)
        {
            Console.WriteLine("\n===== MENU =====");
            Console.WriteLine("1. Create Course");
            Console.WriteLine("2. Modify Course");
            Console.WriteLine("3. Delete Course");
            Console.WriteLine("4. Assign Teacher to Course");
            Console.WriteLine("5. Assign Student to Course");
            Console.WriteLine("6. Show All Courses");
            Console.WriteLine("7. Create Student");
            Console.WriteLine("8. Modify Student");
            Console.WriteLine("9. Delete Student");
            Console.WriteLine("10. Show All Students");
            Console.WriteLine("11. Create Teacher");
            Console.WriteLine("12. Modify Teacher");
            Console.WriteLine("13. Delete Teacher");
            Console.WriteLine("14. Show All Teachers");
            Console.WriteLine("15. Exit");
            Console.Write("Select option (1-15): ");
            
            if (!int.TryParse(Console.ReadLine(), out int choice)) continue;
            
            switch (choice)
            {
                case 1: await CreateEntity("courses"); break;
                case 2: await ModifyEntity("courses"); break;
                case 3: await DeleteEntity("courses"); break;
                case 4: await AssignToCourse("teachers"); break;
                case 5: await AssignToCourse("students"); break;
                case 6: await ShowAll("courses"); break;
                case 7: await CreateEntity("students"); break;
                case 8: await ModifyEntity("students"); break;
                case 9: await DeleteEntity("students"); break;
                case 10: await ShowAll("students"); break;
                case 11: await CreateEntity("teachers"); break;
                case 12: await ModifyEntity("teachers"); break;
                case 13: await DeleteEntity("teachers"); break;
                case 14: await ShowAll("teachers"); break;
                case 15: return;
                default: Console.WriteLine("Invalid option."); break;
            }
        }

        // Shared data types
        public class CourseRecord
        {
            int CId;
            string Name;
            string Department;
            string Description;
            List<int> StudentIds;
            List<int> TeacherIds;
        }

        public class StudentRecord
        {
            int SId;
            string FirstName;
            string LastName;
            string Information;
            List<int> CourseIds;
        }
        public class TeacherRecord
        {
            int TId;
            string FirstName;
            string LastName;
            string Department;
            List<int> CourseIds; 
        }
// Common methods

async Task CreateEntity(string type)
{
    Console.Write($"Enter name/title for new {type[..^1]}: ");
    string? name = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(name)) return;

    object obj = type switch
    {
        "courses" => new Course(0, name, new(), new()),
        "students" => new Student(0, name, new()),
        "teachers" => new Teacher(0, name, new()),
        _ => throw new()
    };

    var res = await client.PostAsJsonAsync(baseUrl + type, obj);
    Console.WriteLine(res.IsSuccessStatusCode ? $"{type[..^1]} created." : $"Error: {res.StatusCode}");
}

async Task ModifyEntity(string type)
{
    Console.Write($"Enter ID of {type[..^1]} to modify: ");
    if (!int.TryParse(Console.ReadLine(), out int id)) return;

    var obj = await client.GetFromJsonAsync<object>($"{baseUrl}{type}/{id}");
    if (obj == null) { Console.WriteLine("Not found."); return; }

    Console.Write("Enter new name/title (leave empty to skip): ");
    string? name = Console.ReadLine();

    object updated = type switch
    {
        "courses" => {
            var c = await client.GetFromJsonAsync<Course>($"{baseUrl}courses/{id}");
            new Course(id, string.IsNullOrWhiteSpace(name) ? c!.Title : name!, c!.StudentIds, c.TeacherIds);
        },
        "students" => {
            var s = await client.GetFromJsonAsync<Student>($"{baseUrl}students/{id}");
            new Student(id, string.IsNullOrWhiteSpace(name) ? s!.Name : name!, s!.CourseIds);
        },
        "teachers" => {
            var t = await client.GetFromJsonAsync<Teacher>($"{baseUrl}teachers/{id}");
            new Teacher(id, string.IsNullOrWhiteSpace(name) ? t!.Name : name!, t!.CourseIds);
        },
        _ => throw new()
    };

    var res = await client.PutAsJsonAsync($"{baseUrl}{type}/{id}", updated);
    Console.WriteLine(res.IsSuccessStatusCode ? "Updated." : $"Error: {res.StatusCode}");
}

async Task DeleteEntity(string type)
{
    Console.Write($"Enter ID of {type[..^1]} to delete: ");
    if (!int.TryParse(Console.ReadLine(), out int id)) return;

    var res = await client.DeleteAsync($"{baseUrl}{type}/{id}");
    Console.WriteLine(res.IsSuccessStatusCode ? "Deleted." : $"Error: {res.StatusCode}");
}

async Task AssignToCourse(string target)
{
    Console.Write($"Enter Course ID: ");
    if (!int.TryParse(Console.ReadLine(), out int cid)) return;

    Console.Write($"Enter {(target == "teachers" ? "Teacher" : "Student")} ID: ");
    if (!int.TryParse(Console.ReadLine(), out int id)) return;

    var res = await client.PutAsync($"{baseUrl}courses/{cid}/{target}/{id}", null);
    Console.WriteLine(res.IsSuccessStatusCode ? "Assigned." : $"Error: {res.StatusCode}");
}

async Task ShowAll(string type)
{
    Console.WriteLine($"\n--- {type.ToUpper()} ---");
    try
    {
        var json = await client.GetStringAsync(baseUrl + type);
        Console.WriteLine(json);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }
}
