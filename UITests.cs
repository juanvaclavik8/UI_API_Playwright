using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;

namespace PlaywrightTests;

[TestClass]
public class UITests : PageTest
{
    private readonly string _urlCanadiens = "https://www.nhl.com/canadiens/roster";
    private readonly string _urlPlayground = "http://uitestingplayground.com/ ";

    [TestMethod]
    public async Task ScrapeRosterCanadiens()
    {
        await Page.GotoAsync(_urlCanadiens);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Page.ClickAsync("#onetrust-accept-btn-handler");

        await Page.WaitForSelectorAsync(".rt-table", new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });

        var rows = await Page.QuerySelectorAllAsync(".rt-table tbody tr");

        var birthPlaces = new List<string>();
        foreach (var row in rows)
        {
            var cell = await row.QuerySelectorAsync("td:nth-child(8)");
            var cellText = await cell.TextContentAsync();
            birthPlaces.Add(cellText.Substring(cellText.Length - 3));
        }

        var canCount = birthPlaces.Count(p => p == "CAN");
        var usaCount = birthPlaces.Count(p => p == "USA");

        Assert.IsTrue(canCount > usaCount, "There is more USA players than CAN players.");
    }

    [TestMethod]
    public async Task SampleAppPage()
    {
        var userName = "sampleLogin";
        var password = "pwd";
        var expected_loginStatus = $"Welcome, {userName}!";
        var expected_logoutStatus = "User logged out.";

        await Page.GotoAsync(_urlPlayground);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Page.ClickAsync("a[href='/sampleapp']");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Page.FillAsync("input[class='form-control'][placeholder='User Name'][type=text]", $"{userName}");
        await Page.FillAsync("input[class='form-control'][type=password]", $"{password}");

        // Login
        await Page.ClickAsync("#login");

        await Page.WaitForSelectorAsync("#loginstatus", new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });

        var received_loginStatus = await Page.TextContentAsync("#loginstatus");

        Assert.AreEqual(received_loginStatus, expected_loginStatus, "Login not successful.");


        // Logout if successfully logged in
        if (await Page.TextContentAsync("#login") == "Log Out")
        {
            await Page.ClickAsync("#login");

            await Page.WaitForSelectorAsync("#loginstatus", new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });

            received_loginStatus = await Page.TextContentAsync("#loginstatus");

            Assert.AreEqual(received_loginStatus, expected_logoutStatus, "Logout not successful.");
        }
    }


    [TestMethod]
    public async Task SampleLoadDelayPage()
    {
        var expected_maxLoadTime = 10000;

        await Page.GotoAsync(_urlPlayground);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Record start time
        DateTime startTime = DateTime.Now;

        await Page.ClickAsync("a[href='/loaddelay']");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Record end time
        DateTime endTime = DateTime.Now;

        TimeSpan received_loadTime = endTime - startTime;

        Assert.IsTrue(received_loadTime.TotalMilliseconds < expected_maxLoadTime, $"Load time exceeded the limit({expected_maxLoadTime}) : {received_loadTime.TotalMilliseconds} ms");
    }

    [TestMethod]
    public async Task SampleProgressBarPage()
    {
        var pbLimit = 75;

        await Page.GotoAsync(_urlPlayground);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Page.ClickAsync("a[href='/progressbar']");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Page.ClickAsync("#startButton");

        string? pbText = null;
        while (true)
        {
            pbText = await Page.TextContentAsync("div#progressBar");
            pbText = pbText?.Replace("%", "");

            int pb;
            int.TryParse(pbText, out pb);

            if (pb >= pbLimit)
            {
                break;
            }
        }

        await Page.ClickAsync("#stopButton");

        var result = await Page.TextContentAsync("p#result");

        TestContext.WriteLine($"Progress Bar stopped: {result}.");
    }
}