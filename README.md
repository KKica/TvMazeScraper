# TvMazeScraper
Scraper of shows and cast from TvMaze api

To run this application you first need to create a sql server database. 

Then apply migrations: Go to the migrations project, fill the required "secret" fields in application.json and run 
  Database-Update from Visual Studio or 
  dotnet ef database update https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli

You also need to put the "secrets" in the main webapi project's application.json
Then you can run the api from visual studio or from the command like.

If you run from visual studio the swagger page will be launched. There are 2 endpoints there.

- Get shows, will return the paginated list of shows, including the cast sorted by birthday descending
- Post will start to fetch data from the TvMaze api. 
  Because of limitations of TvMaze Api, we need to make a lot of http requests to get these information. 
  However in case you stop the app, you will be resuming the fetching of the data and not start over.
  Furthermore only one request at a time is allowed. If you make multiple Post requests only the first one will start scraping. The other ones will fail.

Missing from the solution:

-Integration tests

-Additional unit tests should be added

-Containerization of the application and db spinup using docker

-CI/CD
  

PS: Reasoning for using EF core because its faster to develop, but in no way was it the best choice for the job. Bulk operations is not where EF shines.


