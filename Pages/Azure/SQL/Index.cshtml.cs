using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AzureServices.Pages.Azure.AzureSQL;

[Authorize]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public string Message { get; set; } = string.Empty;

    public void OnGet()
    {
    }

    public void OnPostQuery(string server, string query, string login, string password, string dbname)
    {
        Message = string.Empty;

        try
        {
            var builder = new SqlConnectionStringBuilder();

            builder.DataSource = $"{server}.database.windows.net";
            builder.UserID = login;
            builder.Password = password;
            builder.InitialCatalog = dbname;

            using (var connection = new SqlConnection(builder.ConnectionString))
            {

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Message += string.Format("{0} {1}", reader.GetString(0), reader.GetString(1));
                        }
                    }
                }
            }
        }
        catch (SqlException e)
        {
            Message = e.ToString();
        }
    }
}