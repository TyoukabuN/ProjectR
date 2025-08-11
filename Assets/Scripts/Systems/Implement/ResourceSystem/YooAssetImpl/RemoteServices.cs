using YooAsset;

public class RemoteServices : IRemoteServices
{
    private enum EFormalType
    {
        Full,
        ChannelOnly,
        PlatformOnly,
        NonParam
    }

    private readonly string _defaultHostServer;
    private readonly string _fallbackHostServer;
    private readonly string _channel;
    private readonly string _platform;
    private readonly string _packageName;

    private EFormalType _formatType = EFormalType.NonParam;

    public RemoteServices(string defaultHostServer, string fallbackHostServer, string channel, string platform, string packageName)
    {
        _defaultHostServer = defaultHostServer;
        _fallbackHostServer = fallbackHostServer;
        _channel = channel;
        _platform = platform;
        _packageName = packageName;

        if (!string.IsNullOrEmpty(_channel) && !string.IsNullOrEmpty(_platform))
            _formatType = EFormalType.Full;
        else if (!string.IsNullOrEmpty(_channel))
            _formatType = EFormalType.ChannelOnly;
        else if (!string.IsNullOrEmpty(_platform))
            _formatType = EFormalType.PlatformOnly;
        else
            _formatType = EFormalType.NonParam;
    }
    string IRemoteServices.GetRemoteMainURL(string fileName)
    {
        return GetRemoteURL(_defaultHostServer, fileName);
    }
    string IRemoteServices.GetRemoteFallbackURL(string fileName)
    {
        return GetRemoteURL(_fallbackHostServer, fileName);
    }
    string GetRemoteURL(string server, string fileName)
    {
        switch (_formatType)
        {
            case EFormalType.Full:
                return $"{server}/{_channel}/{_platform}/{_packageName}/{fileName}";
            case EFormalType.ChannelOnly:
                return $"{server}/{_channel}/{_packageName}/{fileName}";
            case EFormalType.PlatformOnly:
                return $"{server}/{_platform}/{_packageName}/{fileName}";
            case EFormalType.NonParam:
                return $"{server}/{_packageName}/{fileName}";
        }
        return $"{server}/{_packageName}/{fileName}";
    }
}