# passthroughpokeapi
Pass-through API to pokeapi.co

This project makes GET or POST requests to pokeapi.co to retrieve basic information regarding Pokemon (their ID number and name). This information is returned as a JSON payload. Each request (successful or not) is logged to a MySQL database running on localhost.

The API is queried via web browser or Postman by running the project within Visual Studio (which will automatically stand up a Kestrel webserver) and connecting to localhost:[port]/myapi/[requestString].

The requestString can either be the Pokemon's name (e.g. "pikachu") or the Pokemon's number (e.g. "25").

This project requires a localhost MySQL database. The code to create the one table used in that database is contained within a multi-line comment in PokemonRequest.cs.
