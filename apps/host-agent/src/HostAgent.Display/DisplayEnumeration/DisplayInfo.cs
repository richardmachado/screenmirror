namespace HostAgent.Display.DisplayEnumeration
{
    public sealed class DisplayInfo
    {
        public string Id { get; }
        public string FriendlyName { get; }
        public string DeviceName { get; }
        public int Width { get; }
        public int Height { get; }
        public int RefreshRate { get; }
        public bool IsPrimary { get; }
        public bool IsVirtualCandidate { get; }

        public DisplayInfo(
            string id,
            string friendlyName,
            string deviceName,
            int width,
            int height,
            int refreshRate,
            bool isPrimary,
            bool isVirtualCandidate)
        {
            Id = id;
            FriendlyName = friendlyName;
            DeviceName = deviceName;
            Width = width;
            Height = height;
            RefreshRate = refreshRate;
            IsPrimary = isPrimary;
            IsVirtualCandidate = isVirtualCandidate;
        }

        public override string ToString()
        {
            return $"{FriendlyName} ({DeviceName}) {Width}x{Height} @{RefreshRate}Hz " +
                   $"Primary={IsPrimary}, VirtualCandidate={IsVirtualCandidate}";
        }
    }
}