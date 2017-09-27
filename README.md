# .Net Data Frames
The .Net Data frames is for performing exploratory data analysis using any .Net language.
The Frame and Column classes inherit from generic lists so all Linq methods work.
All data is stored internally typed.

## Create a new frame and populating with data
   ```csharp
            // Create a new frame
            Frame frame = new Frame();

            // Use the ToColumn extension method to convert an IEnumerable<T> to a Column
            // The collections are stored internally typed
            frame["Names"] = new string[] { "Bob", "Mary", "Joe" }.ToColumn();
            frame["StartDate"] = new string[] { "10/1/2016", "6/8/2016", "9/2/2017" }
                .Select(date => Convert.ToDateTime(date))
                .ToColumn();

            // The static col method allows you quickly create a Column
            frame["Ages"] = col(41.0, 28.0, 35.0);
            frame["HighScore"] = new double[] { 90.0, 92.0, 87.0}.ToColumn();
            frame["LowScore"] = col(78.0, 81.0, 85.0);
   ```
   
   
   ```csharp
   for (int i = 0 ; i < 10; i++)
   {
     // Code to execute.
   }
   ```
   
      
   ```csharp
   for (int i = 0 ; i < 10; i++)
   {
     // Code to execute.
   }
   ```
   
      
   ```csharp
   for (int i = 0 ; i < 10; i++)
   {
     // Code to execute.
   }
   ```
   
      
   ```csharp
   for (int i = 0 ; i < 10; i++)
   {
     // Code to execute.
   }
   ```
   
      
   ```csharp
   for (int i = 0 ; i < 10; i++)
   {
     // Code to execute.
   }
   ```
   
   
   
   Another line
