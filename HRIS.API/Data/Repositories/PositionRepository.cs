using HRIS.API.Entities;

namespace HRIS.API.Data.Repositories;

public class PositionRepository(HrisDbContext context) : Repository<Position>(context), IPositionRepository;