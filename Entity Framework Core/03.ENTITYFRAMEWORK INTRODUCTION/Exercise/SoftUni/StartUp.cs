namespace SoftUni
{
    using SoftUni.Data;
    using SoftUni.Models;

    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using (var context = new SoftUniContext())
            {
                //Console.WriteLine(GetEmployeesFullInformation(context));
                //Console.WriteLine(GetEmployeesWithSalaryOver50000(context));
                //Console.WriteLine(GetEmployeesFromResearchAndDevelopment(context));
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

        //05. Employees from Research and Development
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var departmentName = "Research and Development";

            var sb = new StringBuilder();

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

        //06. Adding a New Address and Updating Employee
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            var addressText = "Vitoshka 15";
            var townId = 4;
            var employeeLastName = "Nakov";

            var sb = new StringBuilder();

            var address = new Address();
            address.AddressText = addressText;
            address.TownId = townId;

            var employeeToChange = context.Employees
                .First(e => e.LastName == employeeLastName);

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

            employees.ForEach(e => sb.AppendLine(e.AddressText));

            return sb.ToString().TrimEnd();
        }

        //07. Employees and Projects
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var startYear = 2001;
            var endYear = 2003;

            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.EmployeesProjects
                            .Any(p => p.Project.StartDate.Year >= startYear &&
                                      p.Project.StartDate.Year <= endYear))
                .Take(10)
                .Select(e => new
                {
                    EmployeeFirstName = e.FirstName,
                    EmployeeLastName = e.LastName,
                    ManagerFirstName = e.Manager.FirstName,
                    ManagerLastName = e.Manager.LastName,
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
                var employeeFullName = $"{employee.EmployeeFirstName} {employee.EmployeeLastName}";
                var managerFullName = $"{employee.ManagerFirstName} {employee.ManagerLastName}";

                sb.AppendLine($"{employeeFullName} - Manager: {managerFullName}");

                foreach (var project in employee.Projects)
                {
                    var startDate = project.StartDate.ToString(format, CultureInfo.InvariantCulture);

                    var endDate = project.EndDate != null
                                                        ? project.EndDate.Value.ToString(format, CultureInfo.InvariantCulture)
                                                        : "not finished";

                    sb.AppendLine($"--{project.Name} - {startDate} - {endDate}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //08. Addresses by Town
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

        //09. Employee 147
        public static string GetEmployee147(SoftUniContext context)
        {
            var employeeId = 147;

            var sb = new StringBuilder();

            var employee = context.Employees
                .Where(e => e.EmployeeId == employeeId)
                .Select(e => new
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    JobTitle = e.JobTitle,
                    Projects = e.EmployeesProjects
                                .Select(p => p.Project.Name)
                                .OrderBy(p => p)
                                .ToList()
                }).First();

            sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");

            var employeeProjects = employee.Projects;

            employeeProjects.ForEach(p => sb.AppendLine(p));

            return sb.ToString().TrimEnd();
        }

        //10. Departments with More Than 5 Employees
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var lessDepartmentsCount = 5;

            var sb = new StringBuilder();

            var departments = context.Departments
                .Where(d => d.Employees.Count > lessDepartmentsCount)
                .OrderBy(d => d.Employees.Count)
                .ThenBy(d => d.Name)
                .Select(d => new
                {
                    DepatmentName = d.Name,
                    ManagerFirstName = d.Manager.FirstName,
                    ManagerLastName = d.Manager.LastName,
                    Employees = d.Employees
					.OrderBy(e => e.EmployeeFirstName)
                    .ThenBy(e => e.EmployeeLastName)
                                .Select(e => new
                                {
                                    EmployeeFirstName = e.FirstName,
                                    EmployeeLastName = e.LastName,
                                    JobTitle = e.JobTitle
                                })
                                
                })
                .ToList();

            foreach (var department in departments)
            {
                var managerFullName = $"{department.ManagerFirstName} {department.ManagerLastName}";
                sb.AppendLine($"{department.DepatmentName} - {managerFullName}");

                foreach (var employee in department.Employees)
                {
                    var emplpoyeeFullName = $"{employee.EmployeeFirstName} {employee.EmployeeLastName}";
                    sb.AppendLine($"{emplpoyeeFullName} - {employee.JobTitle}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //11. Find Latest 10 Projects
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
                .OrderBy(p => p.Name)
                .ToList();

            projects.ForEach(p => sb.AppendLine($"{p.Name}{Environment.NewLine}" +
                                                $"{p.Description}{Environment.NewLine}" +
                                                $"{p.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)}"));

            return sb.ToString().TrimEnd();
        }

        //12. Increase Salaries
        public static string IncreaseSalaries(SoftUniContext context)
        {
            var departments = new string[] { "Engineering", "Tool Design", "Marketing", "Information Services" };

            var sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => departments.Contains(e.Department.Name));

            foreach (var employee in employees)
            {
                employee.Salary *= 1.12M;
            }

            context.SaveChanges();

            employees
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList()
                .ForEach(e => sb.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary:f2})"));

            return sb.ToString().TrimEnd();
        }

        //13. Find Employees by First Name Starting With Sa
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var startingString = "Sa";
            var sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.FirstName.Substring(0, startingString.Length) == startingString) //StartsWith(startingString)
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

        //14. Delete Project by Id
        public static string DeleteProjectById(SoftUniContext context)
        {
            var projectId = 2;

            var sb = new StringBuilder();

            var projectToDelete = context.Projects
                .First(p => p.ProjectId == projectId);

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
                .Take(10)
                .ToList();

            projects.ForEach(p => sb.AppendLine(p));

            return sb.ToString().TrimEnd();
        }

        //15. Remove Town
        public static string RemoveTown(SoftUniContext context)
        {
            var townName = "Seattle";

            var townToDelete = context.Towns
                .First(t => t.Name == townName);

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

            return $"{deletedAddressesCount} addresses in {townName} were deleted";
        }
    }
}
