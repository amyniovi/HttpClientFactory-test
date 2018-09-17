using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using dotnetCoreWordSearch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static System.Console;

namespace WordSearch
{
    //as far as i remember i need to filter the words from this API  search term on title
    //look for multiple word search and length
    //check for null

    //improve dependency injection

    //extract http client use content etc
    //improve speed?
    //general refactoring
    //use tasks instead
    //add logging 
    //add polly
    //add unit testing 

    //https://jsonplaceholder.typicode.com/todos/
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Bootstrap.StartUp();
            Console.WriteLine($"Search titles for these words separated by comma: ");
            var searchTerm = Console.ReadLine();
            var provider = Bootstrap.StartUp();
            var app = new DisplayApp(provider.GetRequiredService<IDataService<ToDo>>(), provider.GetRequiredService<ILoggerFactory>());
            var filteredTitles = await app.GetResults(searchTerm);
            foreach (var title in filteredTitles)
            {
                Console.WriteLine(title);
            }
            //filter results to titles
            //filter titles to required word
            Console.ReadLine();
        }
    }

    public class DisplayApp
    {
        private readonly IDataService<ToDo> _dataService;
        private readonly ILogger _logger;
        private readonly string dataType = "googleSearch";

        public DisplayApp(IDataService<ToDo> dataService, ILoggerFactory logger)
        {
            _dataService = dataService;
            _logger = logger.CreateLogger<DisplayApp>();
        }

        public async Task<List<string>> GetResults(string searchTerm)
        {
            _logger.LogInformation("we are attempting to get the data");
            var list = await _dataService.GetData(dataType);
            if (list == null)
                return new List<string>();
            var titles = list.Select(todo => todo.Title);
            return _dataService.FilterData(searchTerm, titles.ToList());

        }

    }

    public class TodoSearchService<T> : IDataService<T>
    {
        private readonly IHttpClientFactory _clientFactory;

        public TodoSearchService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        public async Task<List<T>> GetData(string dataType)
        {
            List<T> todo = null;

            var client = _clientFactory.CreateClient(dataType);
            var response = await client.GetAsync("");

            if (response.IsSuccessStatusCode)
            {
                todo = response.Content.ReadAsAsync<List<T>>().Result;
            }
            response.StatusCode = HttpStatusCode.RequestTimeout;
            return todo;
        }

    public List<string> FilterData(string search, List<string> titles)
    {
        if (String.IsNullOrEmpty(search))
            return titles;

        string[] words = search.Split(',');

        foreach (var word in words)
        {
            titles = titles.Where(title => title.ToLower().Contains(word.ToLower())).ToList();
        }
        return titles;
    }
}


public interface IDataService<T>
{
    Task<List<T>> GetData(string uri);
    List<string> FilterData(string commaSeparatedSearch, List<string> data);
}

public class ToDo
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; }
    public bool Completed { get; set; }
}

public static class DotNetCoreHelpers
{
    public static async Task<T> ReadAsAsync<T>(this HttpContent content)
    {
        string json = await content.ReadAsStringAsync();
        T value = JsonConvert.DeserializeObject<T>(json);
        return value;
    }
}
}
