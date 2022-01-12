using Microsoft.Identity.Web;
using AzureServices.JsonProperties;
using System.Net.Http.Headers;
using AzureServices.Models;
using Newtonsoft.Json;
using System.Text;
using System.Net;

namespace AzureServices.Services.CarService;

public static class CarServiceExtensions
{
    public static void AddCarService(this IServiceCollection services, IConfiguration configuration)
    {
        // https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
        services.AddHttpClient<ICarService, CarService>();
    }
}

public class CarService : ICarService
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly HttpClient _httpClient;
    private readonly CarServiceProperties properties;
    private readonly ITokenAcquisition _tokenAcquisition;

    public CarService(ITokenAcquisition tokenAcquisition, HttpClient httpClient,
        IConfiguration configuration, IHttpContextAccessor contextAccessor)
    {
        _tokenAcquisition = tokenAcquisition;
        _httpClient = httpClient;
        _contextAccessor = contextAccessor;
        properties = configuration
            .GetSection(nameof(CarServiceProperties))
            .Get<CarServiceProperties>();
    }

    public async Task<Car> AddAsync(Car car)
    {
        await PrepareAuthenticatedClient();

        var jsonRequest = JsonConvert.SerializeObject(car);
        var jsoncontent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        var response = await this._httpClient.PostAsync($"{properties.Address}/cars", jsoncontent);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
            var addedCar = JsonConvert.DeserializeObject<Car>(content);

            return addedCar;
        }

        throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
    }

    public async Task DeleteAllAsync()
    {
        await PrepareAuthenticatedClient();

        var response = await this._httpClient.DeleteAsync($"{properties.Address}/cars/delete-all");
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
        }

        throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
    }

    public async Task DeleteAsync(int id)
    {
        await PrepareAuthenticatedClient();

        var response = await this._httpClient.DeleteAsync($"{properties.Address}/cars/{id}");
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
        }

        throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
    }

    public async Task<IEnumerable<Car>> GetAllAsync()
    {
        await PrepareAuthenticatedClient();

        var response = await this._httpClient.GetAsync($"{properties.Address}/cars/all");
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
            var cars = JsonConvert.DeserializeObject<List<Car>>(content);

            return cars;
        }

        throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
    }

    public async Task<Car> GetAsync(int id)
    {
        await PrepareAuthenticatedClient();

        var response = await this._httpClient.GetAsync($"{properties.Address}/cars/{id}");
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
            var car = JsonConvert.DeserializeObject<Car>(content);

            return car;
        }

        throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
    }

    public async Task<Car> UpdateAsync(int id, Car car)
    {
        await PrepareAuthenticatedClient();

        var jsonRequest = JsonConvert.SerializeObject(car);
        var jsoncontent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        var response = await this._httpClient.PutAsync($"{properties.Address}/cars/{id}", jsoncontent);
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
            var updatedCar = JsonConvert.DeserializeObject<Car>(content);

            return updatedCar;
        }

        throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
    }

    private async Task PrepareAuthenticatedClient()
    {
        var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { properties.Scope });
        Console.WriteLine($"access token-{accessToken}");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
}
