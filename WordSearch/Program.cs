using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;


namespace WordSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            var requiredWrdLength =int.Parse(args[0]);
            var searchWrd = args[1];
            var service = new WordSearchService();

            var task = service.GetRandomWordList();
            task.ContinueWith(t =>
            {
                var filteredList = service.FilterWords(requiredWrdLength, searchWrd, t.Result);
                foreach (var wrd in filteredList)
                    Console.WriteLine(wrd);
            });
            Console.ReadLine();
        }
    }

    public class WordSearchService
    {
        // static due to memory leak with HttpClient 
        //also using would not really work due to the async nature of httpclient (u may dispose the client while you are getting results)
        private static readonly HttpClient HttpClient = new HttpClient();

        public async Task<List<string>> GetRandomWordList()
        {
            var list = new List<string>();
            var response = await HttpClient.GetAsync("https://dl.dropbox.com/u/7543760/wordlist.txt");
            var jsonStr = await response.Content.ReadAsStringAsync();
            //list = JsonConvert.DeserializeObject<List<string>>(jsonStr);
            list = jsonStr.Split('\n')
                .Select(x => x.Trim())
                .ToList();

            return list;
        }


        public IEnumerable<string> FilterWords(int length, string searchTxt, IEnumerable<string> fullList)
        {
            return fullList.Where(wrd => wrd.Length == length && IsSearchTextInWord(wrd, searchTxt));
        }

        private bool IsSearchTextInWord(string word, string searchText )
        {
            int wordIndex = 0;

            foreach (var character in word)
            {
                if (character == searchText[wordIndex] && wordIndex <= searchText.Length - 1)
                {
                    wordIndex++;
                }
            }

            return wordIndex == searchText.Length;

/*
            for (int i = 0; i<searchTxt.Length-1; i++)
            {
                var index = wrd.IndexOf(searchTxt[i]);           
                var otherIndex = wrd.IndexOf(searchTxt[i + 1]);

                if (!(index != -1 && otherIndex != -1 && index <= otherIndex))
                    return false;

            }
            return true;
            
            */
        }

    }
}
