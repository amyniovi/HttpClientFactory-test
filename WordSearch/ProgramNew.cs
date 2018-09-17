using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace WordSearch
{
    //as far as i remember i need to filter the words from this API  search term on title
    //improve dependency injection
    //look for multiple word search and length
    //check for null
    //extract http client use content etc
    //improve speed?
    //https://jsonplaceholder.typicode.com/todos/
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine($"Search titles for: ");
            var searchTerm = Console.ReadLine();
            var service = new TodoSearchService();
            var list = await service.GetData("https://jsonplaceholder.typicode.com/todos/");
            var titles = list.Select(todo => todo.Title);
            var filteredTitles = service.FilterData(searchTerm, titles.ToList());
            foreach (var title in filteredTitles)
            {
                Console.WriteLine(title);
            }
            //filter results to titles
            //filter titles to required word
            Console.ReadLine();
        }
    }

    public class TodoSearchService
    {
        public async Task<List<ToDo>> GetData(string Uri)
        {
            List<ToDo> todo = null;
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(Uri);
                if (response.IsSuccessStatusCode)
                {
                    todo = response.Content.ReadAsAsync<List<ToDo>>().Result;
                }
            }
            return todo;
        }

        public List<string> FilterData(string search, List<string> titles)
        {
            return titles.Where(title => title.ToLower().Contains(search.ToLower())).ToList();
        }
    }

    public class ToDo
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public bool Completed { get; set; }
    }

}
