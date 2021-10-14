using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Constructs;
using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Docker;

namespace CdkDocker
{
    [SuppressMessage("Major Bug", "S1848:Objects should not be created to be dropped immediately without being used", Justification = "By Design")]
    public class Program : TerraformStack
    {
        public Program(Construct scope, string id)
            : base(scope, id)
        {
            new DockerProvider(this, "local");
            new Network(this, "frontend", new NetworkConfig()
            {
                Name = "nm-frontend-vnet",
                IpamConfig = new NetworkIpamConfig[]
                {
                    new NetworkIpamConfig()
                    {
                        Subnet = "172.1.0.0/16",
                    },
                },
            });
        }

        public static void Main(string[] args)
        {
            App app = new App();
            new Program(app, "src");
            app.Synth();
            Console.WriteLine("App synth complete");
        }
    }
}