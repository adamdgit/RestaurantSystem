# BeanScene Restuarant C# project

Steps to run project

1. Install/update packages if needed
2. .NET version 7.0 required
3. Check connection string inside applicationSettings.json, make sure it is linked to your local SQL server
4. Open nuget console, run command update-database (migration file already provided, don't add-migration)
5. Inside Data folder, RestaurantDbContext.cs you will find login credentials for admin and staff accounts.
6. App is ready to go with seeded data.