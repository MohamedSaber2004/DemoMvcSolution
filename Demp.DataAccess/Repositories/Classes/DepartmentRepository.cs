using Demo.DataAccess.Data.Contexts;
using Demo.DataAccess.Models;
using Demo.DataAccess.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DataAccess.Repositories.Classes
{
    public class DepartmentRepository(ApplicationDbContext dbContext) :GenericRepository<Department>(dbContext), IDepartmentRepository

    // Primary Constraint for DI .NET 8 Feature
    // mustn't create any constructor if you make a constructor chaining
    // on primary constructor
    {
        //// Dependency Inversion Principle: high level mustn't
        //// depend on low level model
        //private readonly ApplicationDbContext _dbContext = dbContext;

        //public DepartmentRepository(ApplicationDbContext dbContext) // 1.Injection
        //{
        //    _dbContext = dbContext;
        //}
    }
}
