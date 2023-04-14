using MySql.Data.MySqlClient;
using RestSharp;
using System.Text.Json;

namespace PokemonPassThroughAPI
{
    public class PokemonRequest
    {
        /// <summary>
        /// Connects to the pokemon.co API and attempts to lookup a Pokemon.
        /// </summary>
        /// <param name="requestString">either a Pokemon's number (as string, e.g. "1") or the Pokemon's name</param>
        /// <returns>"Error" if the request was invalid (e.g. negative number or unknown Pokemon), otherwise a deserialized Pokemon JSON object</returns>
        public static string ProcessPokemonRequest(string requestString)
        {
            var client = new RestClient("https://pokeapi.co/api/v2/pokemon/");

            var pokeApiRequest = new RestRequest(requestString);

            var response = client.ExecuteGet(pokeApiRequest);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // Leaving a connection string in code is begrudgingly tolerated.
            // It would be ideal to move it into appsettings.json for several
            // reasons. Unfortunately,the code to do that is in a work project
            // (which I don't feel comfortable accessing for this project), and
            // my tutorial Googling is coming up short.
            // TODO: Move the connection string to appsettings.json.
            string connectionString =
                @"server=localhost;userid=root;password=sup3rs3cur3passw0rd!;database=pokemonRequests";

            using var connection = new MySqlConnection(connectionString);

            connection.Open();

            string nowString = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

            Pokemon pokemon = new Pokemon();

            bool validJson = true;

            try
            {
                pokemon = JsonSerializer.Deserialize<Pokemon>(response.Content!, options)!;
            }

            catch (JsonException)
            {
                validJson = false;
            }

            // The requests table can be created with the following MySQL code:
            /*
            CREATE TABLE `requests` (
            `id` int NOT NULL AUTO_INCREMENT,
            `timestamp` datetime DEFAULT NULL,
            `request` varchar(45) DEFAULT NULL,
            `status` varchar(45) DEFAULT NULL,
            PRIMARY KEY(`id`)
            ) ENGINE = InnoDB AUTO_INCREMENT = 28 DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci;
            */
            string query = "INSERT INTO requests (timestamp, request, status)" +
                "VALUES ('" + nowString + "', '" + requestString + "', '" +
                response.StatusDescription + "')";

            var command = new MySqlCommand(query, connection);

            command.ExecuteScalar();

            if (validJson)
            {
                return JsonSerializer.Serialize<Pokemon>(pokemon);
            }

            else
            {
                return "Error";
            }
        }

        /// <summary>
        /// Queries the pokemon.co API via ProcessPokemonRequest(), and returns an appropriate tuple.
        /// </summary>
        /// <param name="requestString">either a Pokemon number (as string, e.g. "1") or the Pokemon's name</param>
        /// <returns>A tuple consisting of a bool and nullable Pokemon object.
        /// The bool is true if the request succeeded and false otherwise.
        /// The Pokemon object is null if the request failed.</returns>
        public static Tuple<bool, Pokemon?> ReturnResults(string requestString)
        {
            string processedString = ProcessPokemonRequest(requestString);

            return processedString != "Error" ?
                Tuple.Create(true, JsonSerializer.Deserialize<Pokemon>(processedString)) :
                new Tuple<bool, Pokemon?>(false, null);
        }
    }

    public class Pokemon
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}