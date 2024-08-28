using Steamworks;
using System.Net;

namespace SML.Net
{
    public abstract class RemoteAddress
    {
        public abstract string GetIdentifier();
        public abstract string GetFriendlyName();
        public abstract bool IsLocalHost();
    }
    public class TCPIPRemoteAddress : RemoteAddress
    {
        public IPEndPoint IPEndPoint;
        public override string GetFriendlyName() => IPEndPoint.ToString();
        public override string GetIdentifier() => IPEndPoint.ToString();
        public override bool IsLocalHost() => IPEndPoint.Address.Equals(IPAddress.Loopback);
    }
    public class SteamRemoteAddress : RemoteAddress
    {
        public readonly CSteamID SteamId;
        private string _friendlyName;
        public override string ToString()
        {
            string text = (SteamId.m_SteamID % 2uL).ToString();
            string text2 = ((SteamId.m_SteamID - (76561197960265728L + (SteamId.m_SteamID % 2uL))) / 2uL).ToString();
            return "STEAM_0:" + text + ":" + text2;
        }
        public override string GetIdentifier() => ToString();
        public override bool IsLocalHost()
        {
            return SteamAPI.Init() && SteamUser.GetSteamID().Equals(SteamId);
        }
        public override string GetFriendlyName()
        {
            _friendlyName ??= SteamFriends.GetFriendPersonaName(SteamId);
            return _friendlyName;
        }
    }
}
