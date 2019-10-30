namespace SoftUni
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using SoftUni.Data;
    using SoftUni.Models;

    public class StartUp
    {
        public static void Main()
        {
            using (var context = new SoftUniContext())
            {
                //Console.WriteLine(GetEmployeesFullInformation(context));
                //Console.WriteLine(GetEmployeesWithSalaryOver50000(context));
                Console.WriteLine(GetEmployeesFromResearchAndDevelopment(context));
                //Console.WriteLine(AddNewAddressToEmployee(context));
                //Console.WriteLine(GetEmployeesInPeriod(context));
                //Console.WriteLine(GetAddressesByTown(context));
                //Console.WriteLine(GetEmployee147(context));
                //Console.WriteLine(GetDepartmentsWithMoreThan5Employees(context));
                //Console.WriteLine(GetLatestProjects(context));
                //Console.WriteLine(IncreaseSalaries(context));
                //Console.WriteLine(GetEmployeesByFirstNameStartingWithSa(context));
                //Console.WriteLine(DeleteProjectById(context));
                //Console.WriteLine(RemoveTown(context));
            }
        }
        //03. Employees Full Information
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var employees = context.Employees
                .OrderBy(e => e.EmployeeId)
                .Select(e => new
                {
                    e.FirstName, 
                    e.LastName, 
                    e.MiddleName,
                    e.JobTitle,
                    e.Salary
                })
                .ToList();

            employees.ForEach(e => sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:f2}"));

            return sb.ToString().TrimEnd();
        }

        //04. Employees with Salary Over 50 000
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.Salary > 50000)
                .Select(e => new
                {
                    e.FirstName,
                    e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ToList();

            employees.ForEach(e => sb.AppendLine($"{e.FirstName} - {e.Salary:f2}"));

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var sb = new StringBuilder();
            var departmentName = "Research and Development";
            var employees = context.Employees
                .Where(e => e.Department.Name == departmentName)
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.Salary
                })
                .ToList();

            employees.ForEach(e => sb.AppendLine($"{e.FirstName} {e.LastName} from {departmentName} - ${e.Salary:f2}"));

            return sb.ToString().TrimEnd();
        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            var address = new Address();
            address.AddressText = "Vitoshka 15";
            address.TownId = 4;

            var employeeToChange = context.Employees
                .First(e => e.LastName == "Nakov");

            employeeToChange.Address = address;
            context.SaveChanges();

            var employees = context.Employees
                .OrderByDescending(e => e.AddressId)
                .Select(e => new
                {
                    e.Address.AddressText
                })
                .Take(10)
                .ToList();

            var sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine(employee.AddressText);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.EmployeesProjects
                    .Any(p => p.Project.StartDate.Year >= 2001 && p.Project.StartDate.Year <= 2003))
                .Take(10)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    MFirstName = e.Manager.FirstName,
                    MLastName = e.Manager.LastName,
                    Projects = e.EmployeesProjects
                    .Select(p => new
                    {
                        p.Project.Name,
                        p.Project.StartDate,
                        p.Project.EndDate
                    })
                });

            string format = "M/d/yyyy h:mm:ss tt";

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} - Manager: {employee.MFirstName} {employee.MLastName}");

                foreach (var project in employee.Projects)
                {
                    string startDate = project.StartDate.ToString(format, CultureInfo.InvariantCulture);

                    string endDate = project.EndDate == null
                                                        ? "not finished"
                                                        : project.EndDate.Value.ToString(format, CultureInfo.InvariantCulture);                        

                    sb.AppendLine($"--{project.Name} - {startDate} - {endDate}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetAddressesByTown(SoftUniContext context)
        {

            var sb = new StringBuilder();

            var addresses = context.Addresses
                .Select(a => new
                {
                    AddressText = a.AddressText,
                    TownName = a.Town.Name,
                    EmployeesCount = a.Employees.Count
                })
                .OrderByDescending(a => a.EmployeesCount)
                .ThenBy(a => a.TownName)
                .ThenBy(a => a.AddressText)
                .Take(10)
                .ToList();

            addresses.ForEach(a => sb.AppendLine($"{a.AddressText}, {a.TownName} - {a.EmployeesCount} employees"));

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployee147(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var employee = context.Employees
                .Where(e => e.EmployeeId == 147)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    Projects = e.EmployeesProjects
                                .Select(p => p.Project.Name)
                                .OrderBy(p => p)
                                .ToList()
                }).First();

            sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");

            foreach (var project in employee.Projects)
            {
                sb.AppendLine(project);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var departments = context.Departments
                .Where(d => d.Employees.Count > 5)
                .OrderBy(d => d.Employees.Count)
                .ThenBy(d => d.Name)
                .Select(d => new
                {
                    DepatmentName = d.Name,
                    ManagerFirstName = d.Manager.FirstName,
                    ManagerLastName = d.Manager.LastName,
                    Employees = d.Employees
                                .Select(e => new
                                {
                                    EmployeeFirstName = e.FirstName,
                                    EmployeeLastName = e.LastName,
                                    JobTitle = e.JobTitle
                                })
                                .OrderBy(e => e.EmployeeFirstName)
                                .ThenBy(e => e.EmployeeLastName)
                });

            foreach (var department in departments)
            {
                sb.AppendLine($"{department.DepatmentName} - {department.ManagerFirstName} {department.ManagerLastName}");

                foreach (var employee in department.Employees)
                {
                    sb.AppendLine($"{employee.EmployeeFirstName} {employee.EmployeeLastName} - {employee.JobTitle}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetLatestProjects(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var projects = context.Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .Select(p => new
                {
                    p.Name,
                    p.Description,
                    p.StartDate
                })
                .OrderBy(p => p.Name);

            foreach (var project in projects)
            {
                sb.AppendLine($"{project.Name}");
                sb.AppendLine($"{project.Description}");
                sb.AppendLine($"{project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string IncreaseSalaries(SoftUniContext context)
        {
            var sb = new StringBuilder();
            var departments = new string[] { "Engineering", "Tool Design", "Marketing", "Information Services" };

            var employees = context.Employees
                .Where(e => departments.Contains(e.Department.Name));

            foreach (var employee in employees)
            {
                employee.Salary *= 1.12M;
            }

            context.SaveChanges();

            foreach (var employee in employees.OrderBy(e => e.FirstName).ThenBy(e => e.LastName)) 
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} (${employee.Salary:f2})");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.FirstName.Substring(0, 2) == "Sa")
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList();
            employees.ForEach(e => sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:f2})"));

            return sb.ToString().TrimEnd();
        }

        public static string DeleteProjectById(SoftUniContext context)
        {
            var projectToDelete = context.Projects
                .First(p => p.ProjectId == 2);

            var employeesOnSecondProject = context.EmployeesProjects
                .Where(e => e.Project == projectToDelete);

            foreach (var employee in employeesOnSecondProject)
            {
                employee.Project = null;
            }

            context.Projects.Remove(projectToDelete);

            context.SaveChanges();

            var projects = context.Projects
                .Select(p => p.Name)
                .Take(10);

            var sb = new StringBuilder();

            foreach (var project in projects)
            {
                sb.AppendLine(project);
            }

            return sb.ToString().TrimEnd();
        }

        public static string RemoveTown(SoftUniContext context)
        {
            var townToDelete = context.Towns
                .First(t => t.Name == "Seattle");

            var employeesFromTown = context.Employees
                .Where(e => e.Address.Town == townToDelete);

            foreach (var employee in employeesFromTown)
            {
                employee.AddressId = null;
            }

            var addressesFromTown = context.Addresses
                .Where(e => e.Town == townToDelete);

            var deletedAddressesCount = addressesFromTown.Count();

            foreach (var address in addressesFromTown)
            {
                context.Addresses.Remove(address);
            }

            context.Towns.Remove(townToDelete);

            context.SaveChanges();

            return $"{deletedAddressesCount} addresses in Seattle were deleted";
        }
    }
}
