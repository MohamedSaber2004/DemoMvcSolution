
namespace Demo.DataAccess.Repositories.Classes
{
    public class EmployeeRepository(ApplicationDbContext _dbContext):GenericRepository<Employee>(_dbContext), IEmployeeRepository
    {
       
    }
}
