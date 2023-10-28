# QuotebookApp

## Description
This is a quotebook app that friend groups can use to log quotes, store them for free in the cloud, and access them freely on their mobile devices and computers. Google Sheets and the Google Sheets API is used to store the quotebook data in the cloud and to control access to users. To use this software, follow the instructions below to set up the environment with your Google Sheet and then compile the application for all desired targets.

## Current Release
Version 1.0.0 (git tag Release_v1.0.0)

## Prerequisites
The application is compiled using Microsoft's .NET MAUI. .NET Version 7 (minimum) is required to compile the application for iOS, Android, MacCatalyst, and Windows. Note that if you wish to put the app onto iOS devices, a Apple Developer license is required. Additionally, to build for either iOS or MacCatalyst, access to a Mac is required. 

## Setup
1. Create a new Google Sheet using the xlsx template in the root of this repo
2. Follow the steps here to set up your Google Service Account and API: https://support.google.com/a/answer/7378726?hl=en. In step 2, the only API that must be enabled is the Google Sheets v4 API.
3. Open the private key that was downloaded to your computer. The file is in JSON format and must be converted to a C# string. The following template can be used (remove square brackets):
string str = "{\"type\":\"service_account\",\"project_id\":\"[project id goes here]\",\"private_key_id\":\"[private key id goes here]\",\"private_key\":\"-----BEGIN PRIVATE KEY-----\[Very long private key goes here]\n-----END PRIVATE KEY-----\\n\",\"client_email\":\"[email of your service account goes here]\",\"client_id\":\"[client id goes here]\",\"auth_uri\":\"https://accounts.google.com/o/oauth2/auth\",\"token_uri\":\"https://oauth2.googleapis.com/token\",\"auth_provider_x509_cert_url\":\"https://www.googleapis.com/oauth2/v1/certs\",\"client_x509_cert_url\":\[x509 cert url from json file]\",\"universe_domain\":\"googleapis.com\"}"
4. In your google sheet, add your service account as an editor.
5. Pull this repository from GitHub and open the QuotebookApp.sln in Visual Studio.
6. Create a new C# class on the root called ServiceAccount.cs
7. Paste the following code in, replacing my_str with the key string you created in step 3:
namespace QuotebookApp
{
    public class ServiceAccount
    {
        const string GServAcc = my_str;
        public static string GetServAcc()
        {
            return GServAcc;
        }
    }
}
8. Navigate to the directory QuotebookApp/QuotebookApp/Resources/Raw
9. Create a new folder called "Secrets" in this directory
10. Add a file called SheetID.secret and paste the spreadsheet ID in. The Spreadsheet ID is the last string of characters in the URL for your spreadsheet. For example, in the URL https://docs.google.com/spreadsheets/d/1qpyC0XzvTcKT6EISywvqESX3A0MwQoFDE8p-Bll4hps/edit#gid=0, the spreadsheet ID is 1qpyC0XzvTcKT6EISywvqESX3A0MwQoFDE8p-Bll4hps.
11. Add a file called ApplicationName.secret and paste in the name of your application (created in the Google Service Account)
12. Now compile the application for any desired targets

One thing to note is that your private Google Service Account key is embedded in the binary of the application. Though the key is hidden to the average person, it is still possible to find it. This makes it susceptible to hacking if people with knowledge of how to mine out private keys from binaries obtain access to the application. So only share this with your friends, DO NOT publish it to the internet. The reason I designed it this way is because I wanted to avoid having to make a call to a webserver that someone would have to host or pay for. This solution makes the app entirely free and easy to use, but does pose a slight security risk. The good news is, as long as your service account isn't being used to run anything else, the worst a hacker can do is delete all of your quotes.

## Setting up the Spreadsheet
1. The base spreadsheet template has the data set up in the correct columns, etc. but you need to add user information, existing quotes to the spreadsheet.
2. In the Users sheet, create a new user. Currently, users can only be added through the spreadsheet. In the spreadsheet ID put a 1 for the first person, 2 for the second, 3 for third, etc. (the app relies on these indexes) and then the name of the user.
3. Set a generic password for them. Once you're done setting everyone's passwords, be sure to black out the column so you can't see the passwords. Users will be able to update their password in the app once they login. Passwords are supposed to be 4 digits long minumum, so make sure the default password you set meets this criterion. 
4. In the Quotes sheet, add any quotes you would like to start off with. Be sure to follow the formatting used in the examples given in the template.

## Using the App
The Login Page:
This is where users login using the Username and Password from the spreadsheet. A new API call is made every time the user presses the login button to validate the Username and Password against those stored in the cloud.

The Home Page:
This is essentially the navigatin for the app, and internally in .NET MAUI is considered part of the Login Page. Currently, there is button to access the Quotebook, a button to access the Settings, and a button to logout. 

The Quotebook Page:
Here you can scroll through and view quotes. To filter quotes, click the filter button. The filter feature will search for quotes with the specified criteria. If a field is left unspecified, it will not filter that field. To enable/disable the date filter feature, just check/uncheck the checkbox beside the date picker. Click the search button to filter. To restore the original quotes list, just click the Close Filter button. You can also make another API call and refresh the quotes list by clicking Refresh Quotes. To add a quote, click the Add Quote button. You will be required to add at least one person who said the quote (you can add 2 people if it is a dialogue, but it is not required) then enter the quote using quotations or your preferred format in the editor below. Click Add Quote to add it to the spreadsheet. You can return by pressing the back button. 

The Settings Page:
Here a user can update a password by entering their current password and providing a new password for length at least 4.

Themes:
The app has a light and dark theme which are set according to the theme set on the device.


## Contributors
Evan Armoogan - BASc Computer Engineering, University of Waterloo
