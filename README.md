Playwright, MSTest and C# used.

If .NET Playwright environment not set up on local machine, please follow this: https://playwright.dev/dotnet/docs/intro

I wrote some sample UI and API tests in C#. Solution contains two class files with tests.
* UI
  - open web browser and scrape roster of the oldest team and verify there are more Canadian players than players from USA
  - from the Home page, navigate to the Sample App page and cover all the functionalities of that feature by tests
  - on the Home page, click on the Load Delay and verify the page will get loaded in reasonable time
  - from the Home page, navigate to the Progress Bar page and follow the instructions specified in the Scenario section
* API
  - verify the response returned expected count of teams (32 in total)
  - verify the oldest team is Montreal Canadiens
  - verify there's a city with more than 1 team and verify names of those teams
  - verify there are 8 teams in the Metropolitan division and verify them by their names

Solution can be pulled and run in VS2022.
