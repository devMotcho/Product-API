namespace ProductApi
{
    public class ProductDataInjectionService : IHostedService
    {
        private readonly IConfiguration _config;

        public ProductDataInjectionService(IConfiguration config)
        {
            _config = config;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var ProductDataInjection = new ProductDataInjection(_config);
            ProductDataInjection.SetProductData();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}