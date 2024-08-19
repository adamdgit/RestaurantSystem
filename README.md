# Restaurant Management App

This project was created during educational studies as a final project.

Steps to run project

1. Install/update packages if needed
2. .NET version 7.0 required
3. Check connection string inside applicationSettings.json, make sure it is linked to your local SQL server
4. Open nuget console, run command update-database (migration file already provided, don't add-migration)
5. Inside Data folder, RestaurantDbContext.cs you will find login credentials for admin and staff accounts.
6. App is ready to go with seeded data.

## Using The App

Restaurant management app has most of it's functionality and features for admin accounts, where a restaurant owner can manage the details of the reservation system. While logged in as an administrator, you can view all current reservations, tables, areas, sittings and user accounts.

If you are visiting as a customer, you can register an account to create reservations and view your reservation details. Update your profile and view the menu.

## App Features

- View restuarant menu
- Register a user account
- Login to user account with email verification
- Edit user profile details and profile picture
- Create a reservation for the restaurant
- Modify a reservation
- Admin only features
- Table management
- Area management
- Sitting management
- Reservation management
- User account management
- Mobile friendly

### App Preview

Home Screen

![Preview 1](https://github.com/adamdgit/Spotify-Music-player/blob/master/src/screenshots/1.png)

