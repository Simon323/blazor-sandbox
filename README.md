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

## Install validation
```bash
Install-Package MinimalApis.Extensions -Version 0.11.0
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

## Install dotnet-ef
```bash
dotnet tool install --global dotnet-ef --version 8.0.2
```

## Install Microsoft.EntityFrameworkCore.Design
```bash
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.2
```

## List all dotnet tools
```bash
dotnet tool list --global
```

## Add a new migration
```bash
dotnet ef migrations add InitialCreate --output-dir Data\Migrations
```

## Update the database
```bash
dotnet ef database update
```

## Blazor enabled ServerMode
In `Program.cs` add the following code
```csharp
...
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
...
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
```

## Blazor enabled WebAssemblyMode
Install package `Microsoft.AspNetCore.Components.WebAssembly.Server`

In `Program.cs` add the following code
```csharp
...
builder.Services.AddRazorComponents().AddInteractiveWebAssemblyComponents();
...
app.MapRazorComponents<App>().AddInteractiveWebAssemblyRenderMode();
```


## Blazor app
https://youtu.be/RBVIclt4sOo?si=lQRRaROyrd-3Bzjv&t=17793
https://getbootstrap.com/docs/5.3/content/tables/

## Rest API
https://youtu.be/AhAxLiGC7Pc?si=7yUZyNITCa8uGqpk&t=8619

## Blazor SSR / Server
https://youtu.be/LMDo38DPxZc?si=1H202HqvHKCWlGA4&t=11908

## Blazor SSR / Server / WebAssembly
https://youtu.be/C_bYPn-OTtw?si=PS3t5lWGPlAKnPhT&t=509