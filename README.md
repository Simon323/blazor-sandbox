## List dotnet templates
```bash
dotnet new list
```

## Create a new blazor project
```bash
dotnet new blazor --interactivity None --empty -n GameStore.Frontend
```

## Dotnet build
```bash
dotnet build
```

## Dotnet watch
```bash
dotnet watch
```

## Install swagger
```bash
Install-Package Swashbuckle.AspNetCore -Version 6.6.2
```

In `Program.cs` add the following code
```csharp
...
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

...
app.UseSwagger();
app.UseSwaggerUI(c =>
{
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "Game Store API");
	c.EnableTryItOutByDefault();
});
```