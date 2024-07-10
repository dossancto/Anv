using System.Text;
using Cocona;

var app = CoconaApp.Create();

void ParseTokens(AnvTree fatherNode, string[] lines, int depth = 0)
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

void BuildAnvClass(AnvTree tree, StringBuilder sb)
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

app.AddCommand("generate", (string output, string envFile = ".env.example") =>
{
    var env = File.ReadAllText(envFile);

    var tree = new AnvTree()
    {
        Name = "AppEnv"
    };

    var lines = env.Split('\n');


    var sb = new StringBuilder();

    sb.AppendLine("namespace Anv;");

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

    BuildAnvClass(tree, sb);

    var content = sb.ToString();

    var path = Path.Join(output, "AppEnv.cs");

    File.WriteAllText(path, content);
});

app.Run();

public record AnvTree
{
    public List<AnvTree> Nodes { get; set; } = [];

    public bool IsEnv => Nodes.Count == 0 && !string.IsNullOrWhiteSpace(Name);

    public string? Name { get; set; }
    public string? FullName { get; set; }
}
