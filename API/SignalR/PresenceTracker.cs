using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SignalR
{
    ///
    /// Summary
    /// Class to keep track of the presence of a user - wheather he is connected/disconnected, we use a dictionay to achieve this. However this is not scalable and is limited to small no. of users.
    ///
    public class PresenceTracker
    {
        // Use dictionary to store the (username - key) || (List of connectionIds - value)
        private static readonly Dictionary<string, List<string>> OnlineUsers = new Dictionary<string, List<string>>();

        public Task UserConnected(string username, string connectionId)
        {
            // locking the dictionay until we finish using it
            lock (OnlineUsers)
            {
                // if it has the key, we will add the new id to the list
                if (OnlineUsers.ContainsKey(username))
                {
                    OnlineUsers[username].Add(connectionId);
                }
                else
                {
                    OnlineUsers.Add(username, new List<string> { connectionId });
                }
            }
            return Task.CompletedTask;
        }

        // When a user disconnects remove the connectionId from the dictionary
        public Task UserDisconnected(string username, string connectionId)
        {
            lock (OnlineUsers)
            {
                if (!OnlineUsers.ContainsKey(username))
                {
                    return Task.CompletedTask;
                }

                OnlineUsers[username].Remove(connectionId);

                if (OnlineUsers[username].Count == 0)
                {
                    OnlineUsers.Remove(username);
                }
            }
            return Task.CompletedTask;
        }

        // Get collection of usernames who are connected/online
        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUsers;

            lock (OnlineUsers)
            {
                // happens in memory - collect the username and store it in string[] onlineUsers
                onlineUsers = OnlineUsers.OrderBy(k => k.Key)
                                        .Select(k => k.Key)
                                        .ToArray();
            }

            return Task.FromResult(onlineUsers);
        }
    }
}