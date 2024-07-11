using System.Text;
using Anv.Tool;
using static Anv.Tool.Generation;

namespace Anv.Tests;

public class SourceGeneration
{
    [Fact]
    public void TestNameAsync()
    {
        Console.WriteLine("test");

        Console.ReadKey();

        var envFile = File.ReadAllText("../../.././env.test.1");

        // var envFile = """
        //   SALVE.TROPA="eu"
        //   AWS_PROFILE="dev"
        //   SALVE.MEU_REI.SOMETHING="voce"
        //   SALVE.MEU_REI.DAS_NEVES="voce"
        // """.Trim();

        /*
         * 
         * public static partial class AppEnv 
         * {
         *   public static partial class SALVE 
         *   {
         *     public static readonly AnvEnv TROPA = new("SALVE.TROPA"); 
         *     public static readonly AnvEnv MEU_REI = new("SALVE.MEU_REI"); 
         *   }
         *
         * }
         */

        var tree = GenerateTree(envFile);
        var n = tree.Nodes;

        Assert.Equal("SALVE", n[0].Name);

        Assert.Equal("TROPA", n[0].Nodes[0].Name);
        Assert.True(n[0].Nodes[0].IsEnv);
        Assert.Equal("SALVE.TROPA", n[0].Nodes[0].FullName);

        Assert.Equal("MEU_REI", n[0].Nodes[1].Name);
        Assert.False(n[0].Nodes[1].IsEnv);
        Assert.Equal("SALVE.MEU_REI", n[0].Nodes[1].FullName);

        Assert.Equal("SOMETHING", n[0].Nodes[1].Nodes[0].Name);
        Assert.True(n[0].Nodes[1].Nodes[0].IsEnv);
        Assert.Equal("SALVE.MEU_REI.SOMETHING", n[0].Nodes[1].Nodes[0].FullName);

        Assert.Equal("DAS_NEVES", n[0].Nodes[1].Nodes[1].Name);
        Assert.True(n[0].Nodes[1].Nodes[1].IsEnv);
        Assert.Equal("SALVE.MEU_REI.DAS_NEVES", n[0].Nodes[1].Nodes[1].FullName);

        Assert.Equal("AWS_PROFILE", n[1].Name);
        Assert.True(n[1].IsEnv);

        var sb = new StringBuilder();

        sb.AppendLine("namespace Anv;");

        BuildAnvClass(tree, sb);

        Console.WriteLine(sb.ToString());
    }

    [Fact]
    public void DoubleUnderlineSeparator()
    {
        Console.WriteLine("test");

        Console.ReadKey();

        var envFile = File.ReadAllText("../../.././env.test.2");

        // var envFile = """
        //   SALVE__TROPA="eu"
        //   AWS_PROFILE="dev"
        //   SALVE__MEU_REI__SOMETHING="voce"
        //   SALVE__MEU_REI__DAS_NEVES="voce"
        // """.Trim();

        /*
         * 
         * public static partial class AppEnv 
         * {
         *   public static partial class SALVE 
         *   {
         *     public static readonly AnvEnv TROPA = new("SALVE.TROPA"); 
         *     public static readonly AnvEnv MEU_REI = new("SALVE.MEU_REI"); 
         *   }
         *
         * }
         */

        var tree = GenerateTree(envFile, doubleQuoteSeparator: true);
        var n = tree.Nodes;

        Assert.Equal("SALVE", n[0].Name);

        Assert.Equal("TROPA", n[0].Nodes[0].Name);
        Assert.True(n[0].Nodes[0].IsEnv);
        Assert.Equal("SALVE__TROPA", n[0].Nodes[0].FullName);

        Assert.Equal("MEU_REI", n[0].Nodes[1].Name);
        Assert.False(n[0].Nodes[1].IsEnv);
        Assert.Equal("SALVE__MEU_REI", n[0].Nodes[1].FullName);

        Assert.Equal("SOMETHING", n[0].Nodes[1].Nodes[0].Name);
        Assert.True(n[0].Nodes[1].Nodes[0].IsEnv);
        Assert.Equal("SALVE__MEU_REI__SOMETHING", n[0].Nodes[1].Nodes[0].FullName);

        Assert.Equal("DAS_NEVES", n[0].Nodes[1].Nodes[1].Name);
        Assert.True(n[0].Nodes[1].Nodes[1].IsEnv);
        Assert.Equal("SALVE__MEU_REI__DAS_NEVES", n[0].Nodes[1].Nodes[1].FullName);

        Assert.Equal("AWS_PROFILE", n[1].Name);
        Assert.True(n[1].IsEnv);

        var sb = new StringBuilder();

        sb.AppendLine("namespace Anv;");

        BuildAnvClass(tree, sb);

        Console.WriteLine(sb.ToString());
    }

}
