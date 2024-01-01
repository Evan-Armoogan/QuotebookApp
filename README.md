# Quotebook App

## Description
This is a quotebook app that friend groups can use to log quotes, store them for free in the cloud, and access them freely on their mobile devices and computers. Google Sheets and the Google Sheets API is used to store the quotebook data in the cloud and to control access to users. To use this software, follow the instructions below to set up the environment with your Google Sheet and then compile the application for all desired targets.

## Current Release
Version 1.1.0 (git tag Release_v1.1.0)

## Prerequisites
The application is compiled using Microsoft's .NET MAUI. .NET Version 7 (minimum) is required to compile the application for iOS, Android, MacCatalyst, and Windows. Note that if you wish to put the app onto iOS devices, a Apple Developer license is required. Additionally, to build for either iOS or MacCatalyst, access to a Mac is required. 

## Setting up the App
1. Create a new Google Sheet using the xlsx template in the root of this repo
2. Follow the steps here to set up your Google Service Account and API: https://support.google.com/a/answer/7378726?hl=en. In step 2, the only API that must be enabled is the Google Sheets v4 API.
3. Open the private key that was downloaded to your computer. The file is in JSON format and must be converted to a C# string. The following template can be used (remove square brackets):
```string str = "{\"type\":\"service_account\",\"project_id\":\"[project id goes here]\",\"private_key_id\":\"[private key id goes here]\",\"private_key\":\"-----BEGIN PRIVATE KEY-----\[Very long private key goes here]\n-----END PRIVATE KEY-----\\n\",\"client_email\":\"[email of your service account goes here]\",\"client_id\":\"[client id goes here]\",\"auth_uri\":\"https://accounts.google.com/o/oauth2/auth\",\"token_uri\":\"https://oauth2.googleapis.com/token\",\"auth_provider_x509_cert_url\":\"https://www.googleapis.com/oauth2/v1/certs\",\"client_x509_cert_url\":\[x509 cert url from json file]\",\"universe_domain\":\"googleapis.com\"}"```
4. In your google sheet, add your service account as an editor.
5. Pull this repository from GitHub and open the QuotebookApp.sln in Visual Studio.
6. Create a new C# class on the root called ServiceAccount.cs
7. Paste the following code in, replacing my_str with the key string you created in step 3:
```
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
```
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

## Setting up the Quote of the Day Feature
1. A python bot running on a server is required to get the Quote of the Day to work properly. There is a free hosting solution using replit. First open the qotd_bot.py script on the root of this repo. 
2. Go to repl.it and create an account or log in. Click "Create Repl". Select Python as the language and give it any name you want, I just did QOTDBot. Note that this project will be public on your repl.it profile unless you have the premium version, so if you make modifications that you want to keep secret, you must find another hosting solution. 
3. In the main.py file in your new repl, paste the code from the qotd_bot.py file.
4. Navigate to the "Packages" tab (on the Tools section at the bottom left). Install the following packages:
    - Flask
    - google-api-python-client
    - oauth2client
5. Navigate to the "Secrets" tab (on the Tools section at the bottom left). Create a new key called "GServAcc" and paste the JSON contents from your Google Service Account's private key (this was downlaoded to your machine when you created the key in Step 2 of the Setup process). Also add a key called "SpreadsheetID" and paste the ID of your Quotebook spreadsheet (see Step 10 of Setup).
6. Change the TZ_DELTA constant (defined on line 15 of the script) to be the delta between your timezone and UTC.
7. Create two new files and leave them blank:
    - last_qotd_timestamp.txt
    - recent_quotes.txt
8. Create a new file called keep_alive.py. Copy and paste the code from this file into your keep_alive.py: https://replit.com/@EvanBBQ/QOTDBot#keep_alive.py
9. Now run your application by hitting start at the top. A server should boot up. Copy and save the server URL that is shown on the right pane.
10. Now go to https://uptimerobot.com/ and create an account or log in.
11. Click "Add New Monitor" and give your monitor a name (I just did QOTD Bot). Then paste the server URL you saved earlier into the URL box. Set a monitoring interval of 5 minutes. You do not need to use any paid features unless you want to. Press "Save Changes".
12. Now you can close your replit tab and uptimerobot tab. The server may not stay online immediately after you close the replit tab, but given some time (less than an hour) it should come online and stay online. 
13. Now your Quote of the Day bot is all set up and you will see Quote of the Days in the QOTD page in the app.

## Setting up the Reader Feature
1. Go to your Google Developer Cloud Console and open your Quotebook App project (created in Step 2 of the Setup process). Navigate to the Enabled APIs & Services tab. Click on the "ENABLE APIS AND SERVICES" button. Search for and enable the Google Drive API. 
2. On your Google Drive, create a new folder (you can put it anywhere you want). Obtain all of the PDF files you want to be included in the app and upload them to this folder.
3. Share the Google Drive folder that all the PDFs are in with the Google Service Account you created in Step 2 of the Setup process. If you forget the account email, just check which email the spreadsheet is shared with. 
4. Open up the spreadsheet and navigate to the Reader tab. There are 4 columns that must be filled in for each file that you've uploaded to the folder. If a file is in the folder you've shared, but the details are not filled out in the spreadsheet, the app will not have access to that file. To set up a file in the spreadsheet:
    1. Set the File Name parameter to the name you want the file to have when stored locally on users' devices.
    2. Set the Friendly Name parameter to the name you want the file to be displayed as in the app.
    3. Set the File ID parameter to the Google Drive File ID (it works the same as the Spreadsheet ID mentioned in Step 10 of the Setup process). To find this, get the share URL of the file by right clicking > share > copy link. Then obtain the File ID from the URL you copied.
    4. Set the File Size parameter to the size of the file in bytes. You can obtain the size from right clicking on the file > File Information > Details in Google Drive. Be sure to convert whatever number you obtain to bytes.
5. The Reader is now all set up and users can access any of the PDF files directly through the app. 

## Using the App
### The Login Page:
This is where users login using the Username and Password from the spreadsheet. A new API call is made every time the user presses the login button to validate the Username and Password against those stored in the cloud.

### The Home Page:
This is essentially the navigatin for the app, and internally in .NET MAUI is considered part of the Login Page. Currently, there is button to access the Quotebook, a button to access the Settings, and a button to logout. 

### The Quotebook Page:
Here you can scroll through and view quotes. To filter quotes, click the filter button. The filter feature will search for quotes with the specified criteria. If a field is left unspecified, it will not filter that field. To enable/disable the date filter feature, just check/uncheck the checkbox beside the date picker. Click the search button to filter. To restore the original quotes list, just click the Close Filter button. You can also make another API call and refresh the quotes list by clicking Refresh Quotes. To add a quote, click the Add Quote button. You will be required to add at least one person who said the quote (you can add 2 people if it is a dialogue, but it is not required) then enter the quote using quotations or your preferred format in the editor below. If you want to quote somebody who isn't on the app, just choose the Other option in the picker and then type the name of the person in the box below. Click Add Quote to add it to the spreadsheet. You can return by pressing the back button. If you wrote a quote or are quoted in a quote, you have permission to edit/delete that quote. You can do so by double tapping on the quote. You will be brought to the same page as Add Quote but this time you have the option to edit the quote or delete it. 

### The Quote of the Day Page:
This page displays the Quote of the Day that is changed daily by the Quote of the Day bot. You can also get random quotes from the quotebook by selecting the "Get Random Quote" button. 

### The Reader Page:
This page allows you to access various files that the app owner has made available through the app. First click on a file that you want to open. If you don't already have it downloaded, the Download button will become enabled. You can Download the file by clicking it. When a file is downloaded, you can Update it (in case the app owner has updated the file for any reason), Delete it (to remove it from your local disk), and Open it. When you Open the file, a PDF reader will open within the app allowing you to easily read the PDF. 

### The Settings Page:
Here a user can update a password by entering their current password and providing a new password of length at least 4. You can also access the Changelog through the Settings page. 

### App Administrators:
These people are able to access the app while the spreadsheet is down for maintenance and they can edit/delete any quote. More permissions will be added in the future for App Administrators. I recommend keeping this permission limited to the app owner or other trusted individuals. 

### Invisible Users:
These people can access the app, read quotes, access the reader files, etc. but nobody else can see that they have access to the app (i.e. they don't appear in lists of users that show up when quoting people). They also don't have the ability to add quotes themselves. 

### Themes:
The app has a light and dark theme which are set according to the theme set on the device being used.


## Contributors
Evan Armoogan - BASc Computer Engineering, University of Waterloo
