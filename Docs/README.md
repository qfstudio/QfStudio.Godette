# QfStudio.Godette

---

## Nuget cheatsheet

``` shell 
# 添加 Nuget 源
dotnet nuget add source --name lautsky --username lightyears --password PAT_HERE https://lautsky-gitea.qfstudio.net/api/packages/GodotLab/nuget/index.json

# 打包
dotnet pack

# 推送（会自动顺带推送符号包）
dotnet nuget push ".\QfStudio.Godette\bin\Release\QfStudio.Godette.1.0.0.nupkg" --source lautsky
```

### Nuget Package Source Mapping

```shell
# 显示 Nuget 配置路径
dotnet nuget config paths
```

```xml
<!-- Nuget.Config -->
<configuration>
    <!-- ... -->
    <packageSources>
        <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
        <add key="lautsky" value="https://lautsky-gitea.qfstudio.net/api/packages/GodotLab/nuget/index.json" />
    </packageSources>
    <packageSourceMapping>
        <packageSource key="nuget.org">
            <package pattern="*" />
        </packageSource>
        <packageSource key="lautsky">
            <package pattern="QfStudio.*" />
        </packageSource>
    </packageSourceMapping>
</configuration>
```
