# .Net Data Frames
The .Net Data frames is for performing exploratory data analysis using any .Net language.
The Frame and Column classes inherit from generic lists so all Linq methods work.
All data is stored internally typed.

## Getting Started
To install .Net Data Frames, run the following from the Package Manager Console.
```csharp
Install-Package XXXXXXX
```

## Create a new frame and populating with data
   ```csharp
using Spearing.Utilities.Data.Frames;
using static Spearing.Utilities.Data.Frames.FrameExtensions;


    // Create a new frame
    Frame frame = new Frame();

    // Use the ToColumn extension method to convert an IEnumerable<T> to a Column
    // The collections are stored internally typed
    frame["Names"] = new string[] { "Bob", "Mary", "Joe" }.ToColumn();
    frame["StartDate"] = new DateTime[] {
        Convert.ToDateTime("10/1/2016"),
        Convert.ToDateTime("6/8/2016"),
        Convert.ToDateTime("9/2/2017") }
        .ToColumn();

    // The static col method allows you quickly create a Column
    frame["Ages"] = col(41.0, 28.0, 35.0);
    frame["HighScore"] = new double[] { 90.0, 92.0, 87.0}.ToColumn();
    frame["LowScore"] = col(78.0, 81.0, 85.0);
   ```
   
## Displaying data
   ```csharp
    // Print all columns
    frame.Print();

    // Print selected columns
    frame.Print("Names", "ScoreDiff");
   ```
   
## Retrieving data 
   ```csharp
    // The As extension method retrieves the data with typing
    // Make sure to use the type that the column was created with
    double averageAge = frame["Ages"].As<double>().Average();

    double[] ages = frame["Ages"].As<double>().ToArray();
   ```
   
## Column level calculations    
   ```csharp
    frame["ScoreDiff"] = frame["HighScore"].As<double>() - frame["LowScore"].As<double>();
    frame["HighPlus1"] = frame["HighScore"].As<double>() + 1.0;

    frame["Hours"] = new double[] { 25, 30, 38 }.ToColumn();
    double[] hourlyRate = new double[] { 15, 20, 12 };

    frame["Pay"] = frame["Hours"].As<double>() * hourlyRate;
   ```
   
## Saving frame data  
   ```csharp
    frame.SaveCsv(@"c:\temp\Employees.csv");
   ```
   
## Generic lists / collections  
   ```csharp
    public class Employee
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public double HighScore { get; set; }
    }
    
    // Use the ToFrame method to convert a strongly typed collection to a frame
    List<Employee> employees = new List<Employee>()
    {
        new Employee() {Name = "Bob", Age = 40, HighScore = 90.0 },
        new Employee() {Name = "Mary", Age = 28, HighScore = 92.0 },
        new Employee() {Name = "Joe", Age = 35, HighScore = 87.0 }
    };

    Frame employeesFrame = employees.ToFrame();
   ```
         
         
## Filtering     
   ```csharp
    // Use Linq predicates to filter data
    DateTime startDate = Convert.ToDateTime("9/1/2016");
    Frame newEmployees = frame
        .Where(row => row.Get<DateTime>("StartDate") >= startDate)
        .ToFrame(); 
   ```
         
## Grouping     
   ```csharp
    // Group data and use anonymous types to create a new frame
    Frame empYearSummary = frame
        .GroupBy(row => row.Get<DateTime>("StartDate").Year)
        .Select(grp => new
        {
            Year = grp.Key,
            AverageAge = grp.Average(row => row.Get<double>("Ages")),
            Count = grp.Count()
        })
        .ToFrame();
   ```
            
## Populating a frame from a file    
   ```csharp
    // Local file
    Frame employeesLocal = Frame.ReadCSV<string, DateTime, double, double, double, double, double>(@"c:\temp\Employees.csv");

    // Web site
    Frame employeesWeb = Frame.ReadCSV<string, DateTime, double, double, double, double, double>(@"http://www.spearing.com/files/Employees.csv");

    // Git web site
    Frame employeesGit = Frame.ReadCSV<string, DateTime, double, double, double, double, double>(@"https://raw.githubusercontent.com/jackimburgia/Frames/master/Employees.csv");

   ```
            

   
   

