using System;
using Docker.DotNet;
using Docker.DotNet.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace beehive
{
    class Program
    {
        static async Task Main(string[] args)
        {
            DockerClient client = new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock"))
                .CreateClient();
            IList<ContainerListResponse> containers = await client.Containers.ListContainersAsync(new ContainersListParameters
                    {
                        All = true 
                    });

                foreach (var c in containers)
                            Console.WriteLine(JsonConvert.SerializeObject(c));
        }
    }
}
