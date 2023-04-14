using PokemonPassThroughAPI;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/myapi/{request}", (string request) =>
{
    var tuple = PokemonRequest.ReturnResults(request);

    if (tuple.Item1)
    {
        return Results.Ok(tuple.Item2);
    }

    else
    {
        return Results.NotFound();
    }
});

// Code duplication between Get and Post is begrudgingly tolerated.
// Refactoring the duplicated code into a static method returning IResult
// somehow causes the JSON payload to be lost.
// TODO: Look into this further to remove code duplication.
app.MapPost("/myapi/{request}", (string request) =>
{
    var tuple = PokemonRequest.ReturnResults(request);

    if (tuple.Item1)
    {
        return Results.Ok(tuple.Item2);
    }

    else
    {
        return Results.NotFound();
    }
});

app.Run();