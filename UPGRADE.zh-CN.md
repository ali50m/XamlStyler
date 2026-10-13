# .NET 10 升级说明

本次升级以 VS Code 插件及命令行工具为目标。

## 构建范围

- 核心 XamlStyler：net10.0 与 netstandard2.0 双目标。
- CLI 和单元测试：net10.0。
- 使用 .NET 10 稳定 SDK，global.json 的 latestFeature 允许同一大版本内升级 SDK 功能带。
- Directory.Packages.props 集中管理核心、CLI 和测试项目的版本。
- Visual Studio / Rider 宿主项目保留原配置，未纳入本次验证；netstandard2.0 输出保留兼容入口。

## 统一属性管理

根目录 Directory.Build.props 管理核心、CLI 和测试的共享构建属性：

| 属性 | 当前值 / 用途 |
| --- | --- |
| FormatterTargetFramework | net10.0，CLI、测试和核心现代目标共用 |
| CompatibilityTargetFramework | netstandard2.0，仅核心的兼容目标 |
| Version | 3.1.0，底层工具产品及程序集版本 |
| PackageVersion | 跟随 Version，不再单独指定 |
| LangVersion | 14.0 |
| EnableNETAnalyzers / AnalysisLevel | 启用分析器，使用 10.0 规则 |
| Deterministic | 启用确定性编译 |
| Copyright / Company | 共用原项目声明 |

以后更新底层工具版本时修改 Directory.Build.props 的 Version；更新 NuGet 依赖时修改 Directory.Packages.props；选择 SDK 时修改 global.json。

外层 VS Code 插件仍使用 package.json 中的独立版本号。这两个产品版本可以独立发布，不要求数值一致。输出路径、程序集签名、命令行打包和 AOT 等项目特有属性保留在各自 csproj 中。

共享属性只对本次维护的三个项目生效，旧 IDE 宿主不应用这些目标框架或产品版本。

## NuGet 版本

以下为升级时通过 api.nuget.org 查询到的最新稳定版；没有稳定版更新的依赖继续使用原版本。

| 包 | 版本 |
| --- | --- |
| CommandLineParser | 2.9.1 |
| CompareNETObjects | 4.84.0 |
| Irony | 1.5.3 |
| Microsoft.NET.Test.Sdk | 18.10.1 |
| Newtonsoft.Json | 13.0.4 |
| NUnit | 5.0.0 |
| NUnit.ConsoleRunner | 3.22.0 |
| NUnit3TestAdapter | 6.3.1 |
| NuGet.CommandLine | 7.9.0 |

配置仍使用 Newtonsoft.Json，没有迁移 System.Text.Json，也没有改变 Settings.XamlStyler 的配置格式。

## 构建与测试

在此底层仓库根目录执行：

```powershell
dotnet test src/XamlStyler.Cli.slnx
dotnet publish src/XamlStyler.Console/XamlStyler.Console.csproj -c Release -f net10.0 -o ./artifacts/cli
```

生成的 .NET Tool 包在 src/XamlStyler.Console/nupkg。包 ID 沿用 XamlStyler.Console，用于本地测试；如果要公开发布自己的 NuGet 包，先设置自己的唯一 PackageId。

Native AOT：

```powershell
dotnet publish src/XamlStyler.Console/XamlStyler.Console.csproj `
  -c Release -f net10.0 -r win-x64 --self-contained true `
  -p:NativeAot=true -o ./artifacts/aot/win-x64
```

NativeAot 仅对 CLI 启用 PublishAot，同时使用 Size 优化、关闭调试符号、关闭 dotnet tool 打包，并保留原有依赖的反射元数据。不需要临时 props，不要直接把 PublishAot=true 作为全局参数传入。

## 验证结果

- 405 项测试通过，netstandard2.0 兼容目标也正常构建。
- 41 个原有输入样例的托管和 AOT 输出逐字节一致。
- 使用现有 JSON 配置格式化真实语言资源文件三次，后两次完全一致，全部资源值不变，并与已安装的旧版格式化结果一致。
- 本地安装生成的 .NET Tool 后可运行帮助命令。
- 已生成并检查 markupfmt 1.7.0 VSIX，包内运行时为 net10.0；从安装包解压出的工具可独立运行，且未包含 artifacts。
- Native AOT 仍有反射、裁剪相关分析警告。这些实测覆盖的场景通过，不代表全部配置组合都已验证。

测试文件枚举已改为 TestContext 的实际测试目录，并排除生成的 .actual 文件，使 Windows / Linux 使用相同路径逻辑，测试数量也不受前次输出影响。

## 发布顺序

先提交并推送此底层仓库，再在外层插件仓库提交新的子模块引用。外层使用 .NET 10 SDK 构建工具，插件 1.7.0 要求安装 .NET 10 或更高运行时；独立 AOT EXE 自包含，无需安装该运行时。
