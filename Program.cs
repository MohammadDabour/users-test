using System;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Choose an action:");
        Console.WriteLine("1. Create mock users");
        Console.WriteLine("2. Remove mock users");
        Console.WriteLine("3. Count domains from user emails");

        int choice;
        if (int.TryParse(Console.ReadLine(), out choice))
        {
            switch (choice)
            {
                case 1:
                    var createdUsers = Utils.CreateMockUsers();
                    Console.WriteLine($"Created {createdUsers.Count} users.");
                    break;
                case 2:
                    var removedUsers = Utils.RemoveMockUsers();
                    Console.WriteLine($"Removed {removedUsers.Count} users.");
                    break;
                case 3:
                    var domainCounts = Utils.CountDomainsFromUserEmails();
                    Console.WriteLine("Domain counts:");
                    foreach (var item in (IDictionary<string, object>)domainCounts)
                    {
                        Console.WriteLine($"{item.Key}: {item.Value}");
                    }
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a number.");
        }
    }
}
