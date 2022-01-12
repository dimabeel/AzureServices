using AzureServices.Models;

namespace AzureServices.Services.CarService;

public interface ICarService
{
    Task<IEnumerable<Car>> GetAllAsync();

    Task<Car> GetAsync(int id);

    Task DeleteAsync(int id);

    Task DeleteAllAsync();

    Task<Car> AddAsync(Car car);

    Task<Car> UpdateAsync(int id, Car car);
}
