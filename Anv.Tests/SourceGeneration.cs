using System.Text;

namespace Anv.Tests;

public class SourceGeneration
{

    public record AnvTree
    {
        public List<AnvTree> Nodes { get; set; } = [];

        public bool IsEnv => Nodes.Count == 0 && !string.IsNullOrWhiteSpace(Name);

        public string? Name { get; set; }
        public string? FullName { get; set; }
    }

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

        var tree = new AnvTree()
        {
            Name = "AppEnv"
        };

        var lines = envFile.Split('\n');

        foreach (var line in lines)
        {
            var cleanLine = line.Trim().Split("=").First();

            if (cleanLine.StartsWith("#"))
            {
                continue;
            }

            var tokens = cleanLine.Split(".");

            ParseTokens(tree, tokens);
        }

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

    private void ParseTokens(AnvTree fatherNode, string[] lines, int depth = 0)
    {
        var token = lines.ElementAtOrDefault(depth);

        if (token is null)
        {
            return;
        }

        var node = fatherNode.Nodes.FirstOrDefault(x => x.Name == token);

        if (node is not null)
        {
            ParseTokens(node, lines, depth + 1);

            return;
        }

        fatherNode.Nodes.Add(new AnvTree
        {
            Name = token,
            FullName = string.Join(".", lines.Take(depth + 1))
        });

        var recentlyAdded = fatherNode.Nodes.Last();

        ParseTokens(recentlyAdded, lines, depth + 1);

        return;
    }

    private void BuildAnvClass(AnvTree tree, StringBuilder sb)
    {
        if (string.IsNullOrWhiteSpace(tree.Name))
        {
            return;
        }

        sb.AppendLine($"public static partial class {tree.Name} {{");

        foreach (var n in tree.Nodes)
        {
            if (!n.IsEnv)
            {
                BuildAnvClass(n, sb);
                continue;
            }

            sb.AppendLine($"public static readonly AnvEnv {n.Name} = new(\"{n.FullName}\");");
        }

        sb.AppendLine("}");
    }
}

// output:
// public static partial class AppEnv
// {
//     public static partial class SALVE
//     {
//         public static readonly AnvEnv TROPA = new("SALVE.TROPA");
//         public static partial class MEU_REI
//         {
//             public static readonly AnvEnv SOMETHING = new("SALVE.MEU_REI.SOMETHING");
//             public static readonly AnvEnv DAS_NEVES = new("SALVE.MEU_REI.DAS_NEVES");
//         }
//     }
//     public static readonly AnvEnv AWS_PROFILE = new("AWS_PROFILE");
// }
