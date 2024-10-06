using Newtonsoft.Json;
using RestSharp;

namespace PlaywrightTests
{
    [TestClass]
    public class APITests
    {
        private readonly string _baseUrl = "https://qa-assignment.dev1.whalebone.io/api/teams";
        private RestClient _client;
        private RestRequest _request;
        private List<Team> _teams;

        public APITests()
        {
            _client = new RestClient(_baseUrl);
            _request = new RestRequest("", Method.Get);
            _teams = new List<Team>();
        }


        [TestMethod]
        public async Task Test_CountOfTeams_ReturnsOK()
        {
            _teams = await GetTeamsAsync();

            // Arrange
            var expected_teamsCount = 32;


            // Act
            var received_teamsCount = _teams.Count;  


            // Assert
            Assert.AreEqual(expected_teamsCount, received_teamsCount, $"Received teams count is: {received_teamsCount}, but expected is: {expected_teamsCount}.");
        }

        [TestMethod]
        public async Task Test_OldestTeam_ReturnsOK()
        {
            _teams = await GetTeamsAsync();

            // Arrange
            var expected_oldestTeam = "Montreal Canadiens";


            // Act
            // Take teams, order them by firstYearOfPlay(int) and return the first member
            var received_oldestTeam = _teams.OrderBy(t => t.firstYearOfPlay).First().name; 


            // Assert
            Assert.AreEqual(expected_oldestTeam, received_oldestTeam, $"Received oldest team is: {received_oldestTeam}, but should be: {expected_oldestTeam}.");
        }

        [TestMethod]
        public async Task Test_CityWithTwoTeams_ReturnsOK()
        {
            _teams = await GetTeamsAsync();

            // Arrange
            var expected_teams = new List<string> {
                "New York Islanders",
                "New York Rangers"
            };
            var expected_location = "New York";


            // Act
            var received_teamsGroups = new List<List<Team>>();

            // Select groups of teams with the same location
            // Group teams by location, select goup with more than 1 member, take first member of such group and add it to list, give me list of those members
            received_teamsGroups.AddRange(_teams.GroupBy(t => t.location).Where(g => g.Count() > 1).Select(g => g.ToList()).ToList()); 
            
            // Select first group with 2 teams
            var received_firstGroup = received_teamsGroups.First(g =>g.Count() == 2);
            var received_teams = received_firstGroup.Select(t => t.name).ToList();
            
            // Select first teams location 
            var received_location = received_firstGroup.First().location;


            // Assert
            Assert.AreEqual(received_teamsGroups.Count, 1 , "There is more than one group of teams with the same location.");
            Assert.IsTrue(received_teams.Contains(expected_teams[0]), $"Group does not contain {expected_teams[0]}.");
            Assert.IsTrue(received_teams.Contains(expected_teams[1]), $"Group does not contain {expected_teams[1]}.");
            Assert.AreEqual(received_location,expected_location, $"Received location is: {received_location}, but should be: {expected_location}");
        }

        [TestMethod]
        public async Task Test_MetropolitanDivision_ReturnsOK()
        {
            _teams = await GetTeamsAsync();

            // Arrange
            var expected_metropolitanTeamsCount = 8;
            var expected_metropolitanTeamsNames = new List<string>
            {
                "Carolina Hurricanes",
                "Columbus Blue Jackets",
                "New Jersey Devils",
                "New York Islanders",
                "New York Rangers",
                "Philadelphia Flyers",
                "Pittsburgh Penguins",
                "Washington Capitals"
            };


            // Act
            var received_metropolitanTeams = new List<Team>();

            // Select teams from Metropolitan division
            received_metropolitanTeams.AddRange(_teams.Where(t => t.division.name == "Metropolitan").ToList()); 


            // Assert
            Assert.AreEqual(received_metropolitanTeams.Count, 8, $"Received teams count: {received_metropolitanTeams.Count}, expected: {expected_metropolitanTeamsCount}");
            Assert.IsTrue(CompareCollections(received_metropolitanTeams.Select(t => t.name).ToList(), expected_metropolitanTeamsNames), $"Received metropolitan teams: {string.Join(", ", received_metropolitanTeams.Select(t => t.name))}. Expected: {string.Join(", ", expected_metropolitanTeamsNames)}.");
        }


        /// <summary>
        /// Executes http request, returns deserialized list of teams
        /// </summary>
        /// <returns>
        /// Task<List<Team>>
        /// </returns>
        private async Task<List<Team>> GetTeamsAsync()
        {
            var result = new List<Team>();

            var response = await _client.ExecuteAsync(_request);

            if (response != null && !string.IsNullOrEmpty(response.Content))
            {
                var t = JsonConvert.DeserializeObject<Teams>(response.Content) ?? new Teams();
                result.AddRange(t.teams);
            }

            return result;
        }

        /// <summary>
        /// Compares two collections if contain same members
        /// </summary>
        /// <param name="collection1"></param>
        /// <param name="collection2"></param>
        /// <returns></returns>
        private bool CompareCollections(List<string> collection1, List<string> collection2)
        {
            return collection1.Count == collection2.Count &&
                   !collection1.Except(collection2).Any() &&
                   !collection2.Except(collection1).Any();
        }

        private class Teams
        {
            public List<Team> teams = new List<Team>();
        }

        private class Team
        {
            public string name;
            public string location;
            public int founded;
            public int firstYearOfPlay;
            public Division division;
            public string officialSiteUrl;

            public Team()
            {
                division = new Division();
            }
        }

        private class Division
        {
            public int id;
            public string name;
        }
    }
}
