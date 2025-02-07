using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Vulu;

class Program {
    static void Main() {
        ContainerBuilder container = new ContainerBuilder().UseVuluAndPegh();
        Console.WriteLine("Hello, Zulu!");
    }
}