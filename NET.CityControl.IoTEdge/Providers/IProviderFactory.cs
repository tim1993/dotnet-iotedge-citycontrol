namespace NET.CityControl.IoTEdge.Providers
{
    internal interface IProviderFactory: IDisposable
    {
        Task<IProvider> GetSerialProviderAsync(string mode, CancellationToken cancellationToken);

        void ReleaseLock();
    }
}
