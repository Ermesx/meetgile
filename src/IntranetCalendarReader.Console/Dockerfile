FROM microsoft/dotnet:2.1-runtime-nanoserver-sac2016 AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk-nanoserver-sac2016 AS build
WORKDIR /src
COPY src/IntranetCalendarReader.Console/IntranetCalendarReader.Console.csproj IntranetCalendarReader.Console/
COPY src/IntranetCalendar.Provider/IntranetCalendar.Provider.csproj IntranetCalendar.Provider/
COPY src/IntranetCalendar.Abstractions/Meetgile.Domain.csproj IntranetCalendar.Abstractions/
RUN dotnet restore src/IntranetCalendarReader.Console/IntranetCalendarReader.Console.csproj
COPY . .
WORKDIR /src/IntranetCalendarReader.Console
RUN dotnet build IntranetCalendarReader.Console.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish IntranetCalendarReader.Console.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "IntranetCalendarReader.Console.dll"]
