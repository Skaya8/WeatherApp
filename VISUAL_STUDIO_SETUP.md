# Visual Studio & SSMS Setup Guide for WeatherApp

This guide provides comprehensive step-by-step instructions for setting up and running the WeatherApp project using Visual Studio 2022 and SQL Server Management Studio (SSMS).

## Prerequisites Installation

### 1. Install Visual Studio 2022

1. **Download Visual Studio 2022** from [Microsoft's official site](https://visualstudio.microsoft.com/downloads/)
2. **Choose the edition**:
   - **Community** (Free) - Recommended for learning and personal use
   - **Professional** - For small teams
   - **Enterprise** - For large organizations
3. **During installation, select these workloads**:
   - ✅ **ASP.NET and web development**
   - ✅ **.NET desktop development**
   - ✅ **Data storage and processing** (includes SQL Server tools)
4. **Complete the installation** and restart your computer

### 2. Install SQL Server Management Studio (SSMS)

1. **Download SSMS** from [Microsoft's official site](https://aka.ms/ssms)
2. **Run the installer** and follow the setup wizard
3. **SSMS will be installed** and ready to use

## Database Setup with SSMS

### Step 1: Connect to LocalDB

1. **Launch SSMS**
2. **In the Connect to Server dialog**:
   - **Server name**: `(localdb)\MSSQLLocalDB`
   - **Authentication**: Windows Authentication
   - Click **Connect**

### Step 2: Create the Database

1. **In Object Explorer**, right-click on **Databases**
2. **Select "New Database"**
3. **Enter database name**: `WeatherAppDB`
4. **Click OK**

### Step 3: Execute Complete Database Schema

**Option A: Use the Complete Schema Script (Recommended)**

1. **Right-click on WeatherAppDB** → **New Query**
2. **Open the file**: `Database/WeatherAppDB_Complete_Schema.sql`
3. **Copy the entire content** and paste it into the query window
4. **Click Execute** (or press F5)
5. **Verify the execution** by checking the Messages tab for successful completion

**Option B: Manual Step-by-Step Setup**

If you prefer to execute scripts individually, follow the detailed instructions in `Database/README.md`.

### Step 4: Verify Database Setup

After executing the schema script, verify the setup:

1. **Check Tables**: Expand `WeatherAppDB` → `Tables` in Object Explorer
   - You should see: `Users`, `WeatherSearches`, `WeatherSearchChanges`, `WeatherSearchChangeLog`

2. **Check Stored Procedures**: Expand `WeatherAppDB` → `Programmability` → `Stored Procedures`
   - You should see: `sp_ValidateUser`, `sp_InsertWeatherSearch`, `sp_GetWeatherSearches`, etc.

3. **Verify Test Users**: Execute this query:
   ```sql
   USE WeatherAppDB;
   SELECT Id, Username FROM Users;
   ```

4. **Test Authentication**: Execute this query:
   ```sql
   EXEC sp_ValidateUser 'user1', '12345678';
   ```

## Visual Studio Project Setup

### Step 1: Open the Project

1. **Launch Visual Studio 2022**
2. **Click "Open a project or solution"**
3. **Navigate to the WeatherApp folder**
4. **Select WeatherApp.csproj** and click **Open**

### Step 2: Verify Project Structure

In **Solution Explorer**, you should see:
```
WeatherApp/
├── Controllers/
├── Models/
├── Services/
│   ├── Interfaces/
│   └── Repositories/
├── Views/
├── wwwroot/
├── Database/
├── Program.cs
├── appsettings.json
└── WeatherApp.csproj
```

### Step 3: Check Configuration

1. **Double-click appsettings.json** in Solution Explorer
2. **Verify the connection string**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=WeatherAppDB;Trusted_Connection=True;"
  }
}
```

## Building and Running

### Step 1: Build the Project

1. **Press Ctrl+Shift+B** or go to **Build → Build Solution**
2. **Wait for build to complete** (check Output window)
3. **Ensure no errors** in Error List window

### Step 2: Run the Application

1. **Press F5** (Start Debugging) or **Ctrl+F5** (Start Without Debugging)
2. **If prompted about SSL certificate**, click **Yes**
3. **The application will open** in your default browser at `https://localhost:7261`
4. **If HTTPS doesn't work**, try `http://localhost:5043`
5. **Login with**:
   - Username: `user1` (or any of the test users: user2, user3, user4, user5)
   - Password: `12345678`

## Security Features

### Password Security
- **SHA2_256 hashing**: All passwords are securely hashed using SHA2_256
- **No plain text storage**: Passwords are never stored in plain text
- **Secure authentication**: Login validation uses secure hash comparison

### Change Tracking
- **Comprehensive audit trail**: All weather data modifications are logged
- **User tracking**: Every change is associated with the user who made it
- **Dual logging system**: Both `WeatherSearchChanges` and `WeatherSearchChangeLog` tables

## Debugging Features

### Using Visual Studio Debugger

1. **Set breakpoints** by clicking in the left margin of code
2. **Press F5** to start debugging
3. **Step through code** using:
   - F10 (Step Over)
   - F11 (Step Into)
   - Shift+F11 (Step Out)

### Debugging Database Issues

1. **Open SQL Server Profiler** (if available)
2. **Monitor database calls** in real-time
3. **Use SSMS to query the database** while debugging
4. **Check stored procedure execution** in SSMS

## Common Visual Studio Issues

### Build Errors

1. **Clean Solution**: Right-click solution → Clean Solution
2. **Rebuild Solution**: Right-click solution → Rebuild Solution
3. **Check NuGet packages**: Right-click solution → Restore NuGet Packages
4. **Verify .NET 8.0 SDK**: Ensure you have the correct .NET version installed

### Runtime Errors

1. **Check Output window** for detailed error messages
2. **Use Debug → Windows → Exception Settings** to catch specific exceptions
3. **Check the browser's developer tools** (F12) for JavaScript errors
4. **Verify database connection** in SSMS

### Database Connection Issues

1. **Verify LocalDB is running**:
   ```cmd
   sqllocaldb info
   sqllocaldb start "MSSQLLocalDB"
   ```
2. **Check connection string** in appsettings.json
3. **Test connection in SSMS**
4. **Verify database exists** and contains all tables

## Tips for Development

### Visual Studio Shortcuts

- **Ctrl+Shift+B**: Build Solution
- **F5**: Start Debugging
- **Ctrl+F5**: Start Without Debugging
- **Ctrl+K, Ctrl+D**: Format Document
- **Ctrl+K, Ctrl+C**: Comment Selection
- **Ctrl+K, Ctrl+U**: Uncomment Selection
- **Ctrl+Shift+O**: Open File
- **Ctrl+Tab**: Switch between open files

### SSMS Shortcuts

- **F5**: Execute Query
- **Ctrl+N**: New Query
- **Ctrl+R**: Show/Hide Results
- **Ctrl+T**: Show/Hide Object Explorer
- **Ctrl+Shift+R**: Refresh Object Explorer

### Useful Visual Studio Extensions

1. **SQL Server Integration Services** (if working with SSIS)
2. **GitHub Extension for Visual Studio**
3. **Web Essentials** (for web development)
4. **CodeMaid** (for code cleanup)
5. **SQL Server Data Tools** (for database development)

## Troubleshooting

### Port Conflicts

If you get "address already in use" errors:

1. **Quick Fix**: Double-click `fix_port_issues.bat` to automatically fix port conflicts
2. **Manual Check**: Check what's using the port:
   ```cmd
   netstat -ano | findstr :5043
   ```
3. **Check for existing WeatherApp processes**:
   ```cmd
   tasklist | findstr WeatherApp
   ```
4. **Kill existing processes**:
   ```cmd
   taskkill /IM WeatherApp.exe /F
   ```
5. **Or change ports** in `Properties/launchSettings.json`

### Database Lock Issues

1. **Close all SSMS connections** to the database
2. **Restart LocalDB**:
   ```cmd
   sqllocaldb stop "MSSQLLocalDB"
   sqllocaldb start "MSSQLLocalDB"
   ```
3. **Check for long-running transactions** in SSMS

### Authentication Issues

1. **Verify password hashing**: Ensure passwords are properly hashed using SHA2_256
2. **Check stored procedure**: Verify `sp_ValidateUser` is working correctly
3. **Test authentication manually** in SSMS:
   ```sql
   EXEC sp_ValidateUser 'user1', '12345678';
   ```

### Visual Studio Performance

1. **Disable unnecessary extensions**
2. **Increase Visual Studio memory** in Tools → Options → Environment → General
3. **Use lightweight solution load** for large solutions
4. **Close unused tool windows**

## Database Management

### Backup and Restore

1. **Create regular backups**:
   ```sql
   BACKUP DATABASE WeatherAppDB TO DISK = 'C:\Backups\WeatherAppDB.bak'
   ```

2. **Restore from backup**:
   ```sql
   RESTORE DATABASE WeatherAppDB FROM DISK = 'C:\Backups\WeatherAppDB.bak'
   ```

### Performance Monitoring

1. **Monitor query performance** using SQL Server Profiler
2. **Check index usage** in SSMS
3. **Review execution plans** for slow queries
4. **Monitor change log tables** for growth

## Next Steps

Once the application is running:

1. **Explore the code structure** in Solution Explorer
2. **Set breakpoints** to understand the flow
3. **Modify the code** and see changes in real-time
4. **Use the debugging tools** to understand how the application works
5. **Experiment with different cities** in the weather search
6. **Test the change tracking** by modifying weather records
7. **Explore the database** using SSMS to understand the data structure

## Support

If you encounter issues:

1. **Check the main README.md** for general troubleshooting
2. **Review Database/README.md** for database-specific issues
3. **Use Visual Studio's built-in help** (F1)
4. **Search the Error List** for specific error messages
5. **Check the Output window** for detailed logs
6. **Verify database setup** using the verification queries provided 