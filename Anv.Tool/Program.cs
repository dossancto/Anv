using System.Text;
using Cocona;

using static Anv.Tool.Generation;

var app = CoconaApp.Create();


app.AddCommand("generate", (string output, string envFile = ".env.example", bool doubleQuoteSeparator = false) =>
{
    var env = File.ReadAllText(envFile);

    var tree = GenerateTree(env, doubleQuoteSeparator);

    var sb = new StringBuilder();

    sb.AppendLine("namespace Anv;");

    BuildAnvClass(tree, sb);

    var content = sb.ToString();

    var path = Path.Join(output, "AppEnv.cs");

    File.WriteAllText(path, content);
});

app.Run();

