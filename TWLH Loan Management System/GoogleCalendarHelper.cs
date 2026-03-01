using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace TWLH_Loan_Management_System
{
    public static class GoogleCalendarHelper
    {
        static string[] Scopes = { CalendarService.Scope.CalendarEvents };
        static string ApplicationName = "TWLH Loan Management System";

        private static async Task<CalendarService> GetCalendarServiceAsync()
        {
            UserCredential credential;

            string clientSecretPath = "client_secret_903453084214-j567g6bi4enjlo28kmfmfsrmmq5fkvem.apps.googleusercontent.com.json";
            
            if (!File.Exists(clientSecretPath))
            {
                throw new FileNotFoundException("Google Calendar Client Secret file not found.");
            }

            using (var stream = new FileStream(clientSecretPath, FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true));
            }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            return service;
        }

        public static async Task<bool> ScheduleFollowUpAsync(string summary, string description, DateTime eventDate)
        {
            try
            {
                var service = await GetCalendarServiceAsync();

                Event newEvent = new Event()
                {
                    Summary = summary,
                    Description = description,
                    Start = new EventDateTime()
                    {
                        DateTimeDateTimeOffset = new DateTimeOffset(eventDate),
                        TimeZone = "Asia/Manila",
                    },
                    End = new EventDateTime()
                    {
                        DateTimeDateTimeOffset = new DateTimeOffset(eventDate.AddHours(1)), // 1 hour duration
                        TimeZone = "Asia/Manila",
                    },
                    Reminders = new Event.RemindersData()
                    {
                        UseDefault = false,
                        Overrides = new List<EventReminder> {
                            new EventReminder { Method = "popup", Minutes = 30 },
                            new EventReminder { Method = "email", Minutes = 1440 },
                        }
                    }
                };

                string calendarId = "primary";
                EventsResource.InsertRequest request = service.Events.Insert(newEvent, calendarId);
                Event createdEvent = await request.ExecuteAsync();

                return createdEvent != null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Google Calendar Error: " + ex.Message);
                return false;
            }
        }
    }
}
