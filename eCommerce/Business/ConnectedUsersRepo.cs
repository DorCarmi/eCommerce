using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace eCommerce.Business
{
    public class ConnectedUsersRepo
    {
        
        private Mutex _connectMutex;
        // username to user
        private ConcurrentDictionary<string, IUser> _connectedUsers;
        // token to username
        private ConcurrentDictionary<string, string> _connectedUsersTokens;

        public ConnectedUsersRepo()
        {
            _connectMutex = new Mutex();
            _connectedUsers = new ConcurrentDictionary<string, IUser>();
            _connectedUsersTokens = new ConcurrentDictionary<string, string>();
        }

        public bool TryConnectedUser(string username, string token, IUser user)
        {
            _connectMutex.WaitOne();
            if (!_connectedUsers.TryAdd(username, user))
            { 
                _connectMutex.ReleaseMutex();
                return false;
            }

            _connectedUsersTokens.TryAdd(token, username);
            _connectMutex.ReleaseMutex();
            return true;
        }

        public bool TryGetConnectedUser(string token, out IUser user)
        {
            _connectMutex.WaitOne();
            if (!_connectedUsersTokens.TryGetValue(token, out var username))
            {
                _connectedUsers.Remove(username, out var tUser);
                user = null;
                _connectMutex.ReleaseMutex();
                return false;
            }

            _connectedUsers.TryGetValue(username, out user);
            _connectMutex.ReleaseMutex();
            return true;
        }

        public void RemoveConnectedUserFromToken(string token)
        {
            _connectMutex.WaitOne();
            if (_connectedUsersTokens.Remove(token, out var username))
            {
                _connectedUsers.Remove(username, out var user);
            }
            _connectMutex.ReleaseMutex();
        }
    }
}