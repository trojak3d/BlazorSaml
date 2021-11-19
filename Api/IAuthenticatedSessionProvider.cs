using System;
using System.Threading.Tasks;

namespace Api {
    public interface IAuthenticatedSessionProvider {
        string CurrentLoginName { get; }
        void StartSession(string loginName);
        Task EndSessionAsync();
    }
}
