using Autofac;
using ConnelHooley.DirectoryBackupService.S3.Models;

namespace ConnelHooley.DirectoryBackupService.S3
{
    internal sealed class S3IocModule : Module
    {
        public static S3IocModule Create(AwsDestinationSettings settings) => 
            new S3IocModule(settings);

        private readonly AwsDestinationSettings _destinationSettings;

        private S3IocModule(AwsDestinationSettings destinationSettings) => 
            _destinationSettings = destinationSettings;

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_destinationSettings);
        }
    }
}
