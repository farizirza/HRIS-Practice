using HRIS.API.Entities;
using HRIS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Data.Repositories;

public class DepartmentRepository(HrisDbContext context) : Repository<Department>(context), IDepartmentRepository;