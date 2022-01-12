using AzureServices.Services.CarService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AzureServices.Models;

namespace AzureServices.Pages.Cars;

[Authorize]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly ICarService _carService;

    public IndexModel(ILogger<IndexModel> logger, ICarService carService)
    {
        _logger = logger;
        _carService = carService;
    }

    public List<string> Messages { get; set; } = new List<string>();

    public async Task OnGet(int id)
    {
        Messages.Clear();
        try
        {
            Car car = await _carService.GetAsync(id);
            if (car != null)
            {
                string message = $"Id: {id}, mark: {car.Mark}, model: {car.Model}, owner: {car.Owner}.";
                Messages.Add(message);
            }
        }
        catch (Exception e)
        {
            Messages.Add(e.Message);
        }
    }

    public async Task OnPostGetAll()
    {
        Messages.Clear();
        try
        {
            IEnumerable<Car> cars = await _carService.GetAllAsync();
            if (cars != null)
            {
                int counter = 1;
                foreach (Car car in cars)
                {
                    string message = $"Id: {counter}, mark: {car.Mark}, model: {car.Model}, owner: {car.Owner}.";
                    Messages.Add(message);
                    counter++;
                }
            }
        }
        catch (Exception e)
        {
            Messages.Add(e.Message);
        }
    }

    public async Task OnPostDeleteAll()
    {
        Messages.Clear();
        try
        {
            await _carService.DeleteAllAsync();
            Messages.Add("All cars deleted");
        }
        catch (Exception e)
        {
            Messages.Add(e.Message);
        }
    }

    public async Task OnPostDeleteById(int id)
    {
        Messages.Clear();
        try
        {
            await _carService.DeleteAsync(id);
            Messages.Add($"Car with id {id} was deleted");
        }
        catch (Exception e)
        {
            Messages.Add(e.Message);
        }
    }

    public async Task OnPostUpdate(int id, Car carForUpdate)
    {
        Messages.Clear();
        try
        {
            Car car = await _carService.GetAsync(id);
            string owner = car.Owner;
            carForUpdate.Owner = owner;
            Car updatedCar = await _carService.UpdateAsync(id, carForUpdate);
            string message = $"Mark: {updatedCar.Mark}, model: {updatedCar.Model}, owner: {updatedCar.Owner}.";
            Messages.Add(message);
        }
        catch (Exception e)
        {
            Messages.Add(e.Message);
        }
    }

    public async Task OnPostCreate(Car car)
    {
        Messages.Clear();
        try
        {
            string owner = this?.User?.Identity?.Name ?? "Not authorized user";
            car.Owner = owner;
            Car updatedCar = await _carService.AddAsync(car);
            string message = $"Mark: {updatedCar.Mark}, model: {updatedCar.Model}, owner: {updatedCar.Owner}.";
            Messages.Add(message);
        }
        catch (Exception e)
        {
            Messages.Add(e.Message);
        }
    }

    public void OnPostReset()
    {
        Messages = new List<string>();
    }
}